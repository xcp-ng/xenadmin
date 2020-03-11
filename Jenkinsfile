#!groovy

/* Copyright (c) Citrix Systems Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms,
 * with or without modification, are permitted provided
 * that the following conditions are met:
 *
 * *   Redistributions of source code must retain the above
 *     copyright notice, this list of conditions and the
 *     following disclaimer.
 * *   Redistributions in binary form must reproduce the above
 *     copyright notice, this list of conditions and the
 *     following disclaimer in the documentation and/or other
 *     materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
 * SUCH DAMAGE.
 */

/* Note: the env variables are either Jenkins built-in variables
   or own variables configured at Manage Jenkins > Configure System */

properties([
  [
    $class  : 'BuildDiscarderProperty',
    strategy: [$class: 'LogRotator', numToKeepStr: '10', artifactNumToKeepStr: '10']
  ]
])

node('xencenter') {
  try {

    // The job name should be xencenter-<branding>
    def jobName = "${env.JOB_NAME}".tokenize('/')[0]
    def XC_BRANDING = jobName.split('-')[1]
    println "Branding for ${XC_BRANDING}"

    def GIT_COMMIT_XENADMIN = ''
    def GIT_BRANCH_XENADMIN = ''
    def GLOBAL_BUILD_NUMBER = ''

    stage('Bump global build number') {
      GLOBAL_BUILD_NUMBER = build('xencenter-global-build-number').number
      currentBuild.displayName = "${GLOBAL_BUILD_NUMBER}"
    }

    stage('Clean workspace') {
      deleteDir()
    }

    stage('Checkout sources') {
      checkout([
        $class           : 'GitSCM',
        branches         : scm.branches,
        extensions       : [
          [$class: 'RelativeTargetDirectory', relativeTargetDir: 'xenadmin.git'],
          [$class: 'LocalBranch', localBranch: '**'],
          [$class: 'CleanCheckout'],
          [$class: 'CloneOption', noTags: false, reference: '', shallow: true]
        ],
        userRemoteConfigs: scm.userRemoteConfigs
      ])

      GIT_COMMIT_XENADMIN = bat(
        returnStdout: true,
        script: """
                @echo off
                cd ${env.WORKSPACE}\\xenadmin.git
                git rev-parse HEAD
                """
      ).trim()

      GIT_BRANCH_XENADMIN = bat(
      returnStdout: true,
      script: """
                @echo off
                cd ${env.WORKSPACE}\\xenadmin.git
                git rev-parse --abbrev-ref HEAD
                """
    ).trim()

      GString brandingRemote = "${env.CODE_ENDPOINT}/xs/branding.git"

      def branchExistsOnBranding = bat(
        returnStatus: true,
        script: """git ls-remote --heads ${brandingRemote} | grep ${GIT_BRANCH_XENADMIN}"""
      )
      String branchToCloneOnBranding = (branchExistsOnBranding == 0) ?  "${GIT_BRANCH_XENADMIN}" : 'master'

      bat """git clone -b ${branchToCloneOnBranding} ${brandingRemote} ${env.WORKSPACE}\\branding.git"""

      if ("${XC_BRANDING}" != 'citrix') {

        println "Overwriting Branding folder"
        GString xenadminBrandingRemote = "${env.CODE_ENDPOINT}/xsc/xenadmin-branding.git"

        def branchExistsOnBrand = bat(
          returnStatus: true,
          script: """git ls-remote --heads ${xenadminBrandingRemote} | grep ${GIT_BRANCH_XENADMIN}"""
        )
        String branchToClone = (branchExistsOnBrand == 0) ?  "${GIT_BRANCH_XENADMIN}" : 'master'

        bat """
            git clone -b ${branchToClone} ${xenadminBrandingRemote} ${env.WORKSPACE}\\xenadmin-branding.git
            rmdir /s /q "${env.WORKSPACE}\\xenadmin.git\\Branding"
            xcopy /e /y "${env.WORKSPACE}\\xenadmin-branding.git\\${XC_BRANDING}\\*" "${env.WORKSPACE}\\xenadmin.git\\"
        """
      }
    }

    stage('Download dependencies') {

      def remoteDotnet = readFile("${env.WORKSPACE}\\xenadmin.git\\packages\\DOTNET_BUILD_LOCATION").trim()
      def downloadSpec = readFile("${env.WORKSPACE}\\xenadmin.git\\scripts\\deps-map.json").trim().replaceAll("@REMOTE_DOTNET@", remoteDotnet)

      def server = Artifactory.server('repo')
      server.download(downloadSpec)

      if ("${XC_BRANDING}" == 'citrix') {
        println "Downloading hotfixes."

        def hotFixSpec = readFile("${env.WORKSPACE}\\xenadmin.git\\scripts\\hotfix-map.json").trim()
        server.download(hotFixSpec)
      }
    }

    stage('Run checks') {
      dir("${env.WORKSPACE}\\xenadmin.git") {
        powershell ".\\scripts\\check_copyright.ps1"
        powershell ".\\scripts\\check_i18n.ps1"
      }
    }

    stage('Build') {
      def sbe = "${GIT_BRANCH_XENADMIN}".startsWith('release').toString().toLowerCase()

      bat """
cd ${env.WORKSPACE}
sh xenadmin.git/scripts/xenadmin-build.sh ${GLOBAL_BUILD_NUMBER} ${env.SIGNING_NODE_NAME} ${sbe} ${env.SELFSIGN_THUMBPRINT_SHA1} ${env.SELFSIGN_THUMBPRINT_SHA256} ${env.TIMESTAMP_SERVER_URL}
      """
    }

    stage('Create manifest') {
      GString manifestFile = "${env.WORKSPACE}\\xenadmin.git\\_output\\xenadmin-manifest.txt"

      def brandingTip = bat(
        returnStdout: true,
        script: """
                @echo off
                cd ${env.WORKSPACE}\\branding.git
                git rev-parse HEAD
                """
      ).trim()

      powershell """
"xenadmin xenadmin.git ${GIT_COMMIT_XENADMIN}" | Out-File -FilePath ${manifestFile}
"branding branding.git ${brandingTip}" | Out-File -FilePath ${manifestFile} -Append
"""

      if ("${XC_BRANDING}" != 'citrix') {
        def xenadminBrandingTip = bat(
          returnStdout: true,
          script: """
                @echo off
                cd ${env.WORKSPACE}\\xenadmin-branding.git
                git rev-parse HEAD
                """
        ).trim()

        powershell """
"xenadmin-branding xenadmin-branding.git" ${xenadminBrandingTip} | Out-File -FilePath ${manifestFile} -Append
"""
      }

      def dotNetManifest = readFile("${env.WORKSPACE}\\xenadmin.git\\packages\\dotnet-packages-manifest.txt").trim()

      powershell """
"xencenter-ovf xencenter-ovf.git 21d3d7a7041f15abfa73f916e5fd596fd7e610c4" | Out-File -FilePath ${manifestFile} -Append
"chroot-lenny chroots.hg 1a75fa5848e8" | Out-File -FilePath ${manifestFile} -Append
"${dotNetManifest}" | Out-File -FilePath ${manifestFile} -Append
"""
    }

    stage('Run tests') {
      timeout(time: 60, unit: 'MINUTES') {
        bat """
taskkill /f /fi "imagename eq nunit*"

nunit3-console /labels=all /process=separate /timeout=40000 /where "cat==Unit" ^
  /out="${env.WORKSPACE}\\xenadmin.git\\_output\\XenAdminTests.out" ^
  /result="${env.WORKSPACE}\\xenadmin.git\\_output\\XenAdminTests.xml" ^
  "${env.WORKSPACE}\\xenadmin.git\\XenAdminTests\\bin\\Release\\XenAdminTests.dll" ^
  > ${env.WORKSPACE}\\xenadmin.git\\_output\\nunit3-console.out

type ${env.WORKSPACE}\\xenadmin.git\\_output\\nunit3-console.out
"""

        def text = readFile("${env.WORKSPACE}\\xenadmin.git\\_output\\nunit3-console.out")
        assert text.contains('Failed: 0')
      }
    }

    stage('Upload') {
      dir("${env.WORKSPACE}\\xenadmin.git\\_output") {

        def server = Artifactory.server('repo')
        def buildInfo = Artifactory.newBuildInfo()
        buildInfo.env.filter.addInclude("*")
        buildInfo.env.collect()

        GString artifactMeta = "build.name=${env.JOB_NAME};build.number=${GLOBAL_BUILD_NUMBER};vcs.url=${env.CHANGE_URL};vcs.branch=${GIT_BRANCH_XENADMIN};vcs.revision=${GIT_COMMIT_XENADMIN}"

        // IMPORTANT: do not forget the slash at the end of the target path!
        GString targetPath = "xc-local-build/xencenter/${GIT_BRANCH_XENADMIN}/${XC_BRANDING}/${GLOBAL_BUILD_NUMBER}/"
        GString uploadSpec = """ {
            "files": [
              { "pattern": "*", "flat": "false", "target": "${targetPath}", "props": "${artifactMeta}" }
            ]}
        """

        def buildInfo_upload = server.upload(uploadSpec)
        buildInfo.append buildInfo_upload
        server.publishBuildInfo buildInfo
      }
    }

    currentBuild.result = 'SUCCESS'

  } catch (Exception ex) {
    currentBuild.result = 'FAILURE'
    throw ex as java.lang.Throwable
  } finally {
    step([
      $class                  : 'Mailer',
      notifyEveryUnstableBuild: true,
      recipients              : "${env.XENCENTER_DEVELOPERS}",
      sendToIndividuals       : true])
  }
}
