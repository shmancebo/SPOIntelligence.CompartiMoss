using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CongnitveManager
{
    public class FaceRectangleCelebrity
    {
        public int left { get; set; }
        public int top { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class FaceRectangle
    {
        public int height { get; set; }
        public int left { get; set; }
        public int top { get; set; }
        public int width { get; set; }
    }


    public class Celebrity
    {
        public string name { get; set; }
        public FaceRectangleCelebrity faceRectangle { get; set; }
        public double confidence { get; set; }
    }

    public class Tags
    {
        List<string> tags { get; set; }
    }

    public class Detail
    {
        public List<Celebrity> celebrities { get; set; }
    }

    public class Description
    {
        public List<string> tags { get; set; }
        public dynamic captions { get; set; }
    }

    public class Category
    {
        public string name { get; set; }
        public double score { get; set; }
        public Detail detail { get; set; }
    }

    public class Metadata
    {
        public int width { get; set; }
        public int height { get; set; }
        public string format { get; set; }
    }

    public class Scores
    {
        public double anger { get; set; }
        public double contempt { get; set; }
        public double disgust { get; set; }
        public double fear { get; set; }
        public double happiness { get; set; }
        public double neutral { get; set; }
        public double sadness { get; set; }
        public double surprise { get; set; }
    }

    public class RootEmotionObject
    {
        public FaceRectangle faceRectangle { get; set; }
        public Scores scores { get; set; }
    }

    public class RootCelebrityObject
    {
        public List<Category> categories { get; set; }
        public Description description { get; set; }
        public string requestId { get; set; }
        public Metadata metadata { get; set; }
    }

    public class ContentResult
    {
        public Celebrity celebrity { get; set; }
        public List<string> tags { get; set; }
    }

}

