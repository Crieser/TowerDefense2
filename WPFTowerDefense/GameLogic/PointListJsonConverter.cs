using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace WPFTowerDefense.GameLogic
{
    // JSON converter for a list of Point objects
    public class PointListJsonConverter : JsonConverter<List<Point>>
    {
        public override List<Point> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var points = new List<Point>();

            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                if (reader.TokenType != JsonTokenType.StartArray)
                    throw new JsonException();

                reader.Read();
                double x = reader.GetDouble();
                reader.Read();
                double y = reader.GetDouble();

                reader.Read();
                points.Add(new Point(x, y));
            }

            return points;
        }

        public override void Write(Utf8JsonWriter writer, List<Point> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var point in value)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue(point.X);
                writer.WriteNumberValue(point.Y);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }
    }
}
