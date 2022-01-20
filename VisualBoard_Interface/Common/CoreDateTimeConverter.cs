using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VisualBoard_Interface.Common
{
    public class CoreDateTimeConverter : JsonConverter<DateTime?>
    {
        /// <summary>
        /// 获取或设置DateTime格式
        /// <para>默认为: yyyy-MM-dd HH:mm:ss</para>
        /// </summary>
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => string.IsNullOrEmpty(reader.GetString()) ? default(DateTime?) : DateTime.Parse(reader.GetString());

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
            => writer.WriteStringValue(value?.ToString(this.DateTimeFormat));
    }

    public class CoreIntConverter : JsonConverter<int>
    {

        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                return reader.GetInt32();
            }
            catch { return 0; }
        }
        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
         => writer.WriteNumberValue(value);
    }

    public class CoreStringConverter : JsonConverter<string>
    {

        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                return reader.GetString();
            }
            catch { return ""; }
        }
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
         => writer.WriteStringValue(value);
    }
}

