using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Sitefinity_Web.Mvc.Models.ImagePostData;


namespace Sitefinity_Web.Mvc.Models
{
    public class ImagePostData
    {

        public class ImagePostApi
        {
            public ImageApi[] Property1 { get; set; }
        }

        public class ImageApi
        {
            public int id { get; set; }
            public DateTime date { get; set; }
            public DateTime date_gmt { get; set; }
            public Guid guid { get; set; }
            public DateTime modified { get; set; }
            public DateTime modified_gmt { get; set; }
            public string slug { get; set; }
            public string status { get; set; }
            public string type { get; set; }
            public string link { get; set; }
            public Title title { get; set; }
            public int author { get; set; }
            public string comment_status { get; set; }
            public string ping_status { get; set; }
            public string template { get; set; }
            public Meta meta { get; set; }
            public object[] acf { get; set; }

            //[Newtonsoft.Json.JsonConverter(typeof(SmushConverter))]
            //public Smush smush { get; set; }
            public Description description { get; set; }
            public Caption caption { get; set; }
            public string alt_text { get; set; }
            public string media_type { get; set; }
            public string mime_type { get; set; }
            public Media_Details media_details { get; set; }
            public int post { get; set; }
            public string source_url { get; set; }
            public _Links _links { get; set; }
        }

        public class Guid
        {
            public string rendered { get; set; }
        }

        public class Title
        {
            public string rendered { get; set; }
        }

        public class Meta
        {
            public bool _acf_changed { get; set; }
            public bool _monsterinsights_skip_tracking { get; set; }
            public bool _monsterinsights_sitenote_active { get; set; }
            public string _monsterinsights_sitenote_note { get; set; }
            public int _monsterinsights_sitenote_category { get; set; }
        }

        
        //public class Smush
        //{
        //    public string Status { get; set; }
        //    public Stats stats { get; set; }
        //    public Sizes sizes { get; set; }
        //}

        //public class Stats
        //{
        //    public float percent { get; set; }
        //    public int bytes { get; set; }
        //    public int size_before { get; set; }
        //    public int size_after { get; set; }
        //    public float time { get; set; }
        //    public string api_version { get; set; }

        //    [Newtonsoft.Json.JsonConverter(typeof(FlexibleBooleanConverter))]
        //    public bool lossy { get; set; }

       
        //    public int keep_exif { get; set; }
        //}

        //public class Sizes
        //{
        //    public Medium medium { get; set; }
        //    public Large large { get; set; }
        //    public Thumbnail thumbnail { get; set; }
        //    public Medium_Large medium_large { get; set; }
        //    public _1536X1536 _1536x1536 { get; set; }
        //    public _2048X2048 _2048x2048 { get; set; }
        //    public Wp_Scaled wp_scaled { get; set; }
        //}

        public class Medium
        {
            public float percent { get; set; }
            public int bytes { get; set; }
            public int size_before { get; set; }
            public int size_after { get; set; }
            public float time { get; set; }
        }

        public class Large
        {
            public float percent { get; set; }
            public int bytes { get; set; }
            public int size_before { get; set; }
            public int size_after { get; set; }
            public float time { get; set; }
        }

        public class Thumbnail
        {
            public float percent { get; set; }
            public int bytes { get; set; }
            public int size_before { get; set; }
            public int size_after { get; set; }
            public float time { get; set; }
        }

        public class Medium_Large
        {
            public float percent { get; set; }
            public int bytes { get; set; }
            public int size_before { get; set; }
            public int size_after { get; set; }
            public float time { get; set; }
        }

        public class _1536X1536
        {
            public float percent { get; set; }
            public int bytes { get; set; }
            public int size_before { get; set; }
            public int size_after { get; set; }
            public float time { get; set; }
        }

        public class _2048X2048
        {
            public float percent { get; set; }
            public int bytes { get; set; }
            public int size_before { get; set; }
            public int size_after { get; set; }
            public float time { get; set; }
        }

        public class Wp_Scaled
        {
            public float percent { get; set; }
            public int bytes { get; set; }
            public int size_before { get; set; }
            public int size_after { get; set; }
            public float time { get; set; }
        }

        public class Description
        {
            public string rendered { get; set; }
        }

        public class Caption
        {
            public string rendered { get; set; }
        }

        public class Media_Details
        {
            public int width { get; set; }
            public int height { get; set; }
            public string file { get; set; }
            public int filesize { get; set; }
            public Sizes1 sizes { get; set; }
            public Image_Meta image_meta { get; set; }
            public string original_image { get; set; }
        }

        public class Sizes1
        {
            public Medium1 medium { get; set; }
            public Large1 large { get; set; }
            public Thumbnail1 thumbnail { get; set; }
            public Medium_Large1 medium_large { get; set; }
            public _1536X15361 _1536x1536 { get; set; }
            public _2048X20481 _2048x2048 { get; set; }
            public Full full { get; set; }
        }

        public class Medium1
        {
            public string file { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int filesize { get; set; }
            public string mime_type { get; set; }
            public string source_url { get; set; }
        }

        public class Large1
        {
            public string file { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int filesize { get; set; }
            public string mime_type { get; set; }
            public string source_url { get; set; }
        }

        public class Thumbnail1
        {
            public string file { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int filesize { get; set; }
            public string mime_type { get; set; }
            public string source_url { get; set; }
        }

        public class Medium_Large1
        {
            public string file { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int filesize { get; set; }
            public string mime_type { get; set; }
            public string source_url { get; set; }
        }

        public class _1536X15361
        {
            public string file { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int filesize { get; set; }
            public string mime_type { get; set; }
            public string source_url { get; set; }
        }

        public class _2048X20481
        {
            public string file { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int filesize { get; set; }
            public string mime_type { get; set; }
            public string source_url { get; set; }
        }

        public class Full
        {
            public string file { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public string mime_type { get; set; }
            public string source_url { get; set; }
        }

        public class Image_Meta
        {
            public string aperture { get; set; }
            public string credit { get; set; }
            public string camera { get; set; }
            public string caption { get; set; }
            public string created_timestamp { get; set; }
            public string copyright { get; set; }
            public string focal_length { get; set; }
            public string iso { get; set; }
            public string shutter_speed { get; set; }
            public string title { get; set; }
            public string orientation { get; set; }
            public object[] keywords { get; set; }
        }

        public class _Links
        {
            public Self[] self { get; set; }
            public Collection[] collection { get; set; }
            public About[] about { get; set; }
            public Author[] author { get; set; }
            public Reply[] replies { get; set; }
        }

        public class Self
        {
            public string href { get; set; }
        }

        public class Collection
        {
            public string href { get; set; }
        }

        public class About
        {
            public string href { get; set; }
        }

        public class Author
        {
            public bool embeddable { get; set; }
            public string href { get; set; }
        }

        public class Reply
        {
            public bool embeddable { get; set; }
            public string href { get; set; }
        }

    }

    //public class FlexibleBooleanConverter : Newtonsoft.Json.JsonConverter
    //{
    //    public override bool CanConvert(Type objectType)
    //    {
    //        return objectType == typeof(bool);
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        if (reader.TokenType == JsonToken.Boolean)
    //        {
    //            return reader.Value;
    //        }
    //        if (reader.TokenType == JsonToken.Integer)
    //        {
    //            int intValue = Convert.ToInt32(reader.Value);
    //            return intValue == 1;
    //        }
    //        if (reader.TokenType == JsonToken.String)
    //        {
    //            if (bool.TryParse((string)reader.Value, out bool result))
    //            {
    //                return result;
    //            }
    //            if (int.TryParse((string)reader.Value, out int intResult))
    //            {
    //                return intResult == 1;
    //            }
    //        }
    //        throw new JsonReaderException($"Unexpected token {reader.TokenType} when parsing boolean.");
    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        writer.WriteValue(value);
    //    }
    //}

    //public class SmushConverter : Newtonsoft.Json.JsonConverter
    //{
    //    public override bool CanConvert(Type objectType)
    //    {
    //        return objectType == typeof(Smush);
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        if (reader.TokenType == JsonToken.StartObject)
    //        {
    //            return serializer.Deserialize<Smush>(reader);
    //        }
    //        if (reader.TokenType == JsonToken.String)
    //        {
    //            return new Smush { Status = reader.Value.ToString() };
    //        }
    //        throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing smush.");
    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        var smush = value as Smush;
    //        if (smush != null)
    //        {
    //            if (!string.IsNullOrEmpty(smush.Status))
    //            {
    //                writer.WriteValue(smush.Status);
    //            }
    //            else
    //            {
    //                serializer.Serialize(writer, value);
    //            }
    //        }
    //    }
    //}
}