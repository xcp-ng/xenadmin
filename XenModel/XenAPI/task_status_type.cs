/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 *   1) Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *
 *   2) Redistributions in binary form must reproduce the above
 *      copyright notice, this list of conditions and the following
 *      disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using Newtonsoft.Json;


namespace XenAPI
{
    [JsonConverter(typeof(task_status_typeConverter))]
    public enum task_status_type
    {
        /// <summary>
        /// task is in progress
        /// </summary>
        pending,
        /// <summary>
        /// task was completed successfully
        /// </summary>
        success,
        /// <summary>
        /// task has failed
        /// </summary>
        failure,
        /// <summary>
        /// task is being cancelled
        /// </summary>
        cancelling,
        /// <summary>
        /// task has been cancelled
        /// </summary>
        cancelled,
        unknown
    }

    public static class task_status_type_helper
    {
        public static string ToString(task_status_type x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this task_status_type x)
        {
            switch (x)
            {
                case task_status_type.pending:
                    return "pending";
                case task_status_type.success:
                    return "success";
                case task_status_type.failure:
                    return "failure";
                case task_status_type.cancelling:
                    return "cancelling";
                case task_status_type.cancelled:
                    return "cancelled";
                default:
                    return "unknown";
            }
        }
    }

    internal class task_status_typeConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((task_status_type)value).StringOf());
        }
    }
}