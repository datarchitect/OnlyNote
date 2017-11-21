using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace EmotionAPI
{
    class Face
    {
        int _height; int _left; int _top; int _width;

        public Face() { }

        public int height { get => _height; set => _height = value; }
        public int left { get => _left; set => _left = value; }
        public int top { get => _top; set => _top = value; }
        public int width { get => _width; set => _width = value; }
    }
    class Score
    {
        double _anger;
        double _contempt;
        double _disgust;
        double _fear;
        double _happiness;
        double _neutral;
        double _sadness;
        double _surprise;

        public Score() { }
        //public EmotionOutput(double anger,
        //                        double contempt,
        //                        double disgust,
        //                        double fear,
        //                        double happiness,
        //                        double neutral,
        //                        double sadness,
        //                        double surprise)
        //{
        //    this.Anger = anger;
        //    this.Contempt = contempt;
        //    this.Disgust = disgust;
        //    this.Fear = fear;
        //    this.Happiness = happiness;
        //    this.Neutral = neutral;
        //    this.Sadness = sadness;
        //    this.Surprise = surprise;

        //}

        public double anger { get => _anger; set => _anger = value; }
        public double contempt { get => _contempt; set => _contempt = value; }
        public double disgust { get => _disgust; set => _disgust = value; }
        public double fear { get => _fear; set => _fear = value; }
        public double happiness { get => _happiness; set => _happiness = value; }
        public double neutral { get => _neutral; set => _neutral = value; }
        public double sadness { get => _sadness; set => _sadness = value; }
        public double surprise { get => _surprise; set => _surprise = value; }
    }

    class EmotionChart
    {
        public Face faceRectangle;
        public Score scores;
    }

    class EmoList : List<EmotionChart> 
        { }

    class deser
    {
        public void ParseJSON()
        {
            string jsonStr = @"[{'faceRectangle':{'height':261,'left':140,'top':230,'width':261},'scores':{'anger':0.0268293936,'contempt':0.130030066,'disgust':0.0107496157,'fear':0.000155375863,'happiness':0.08784503,'neutral':0.72912854,'sadness':0.0145995952,'surprise':0.000662396953}}]";
            JavaScriptSerializer jss = new JavaScriptSerializer();

            var obj = jss.Deserialize(jsonStr, typeof(Object));
            EmoList emoList = obj as EmoList;
        }

        public string ConvertToJSON()
        {
            Score emo = new Score();
            emo.anger = 0.0268293936;
            emo.contempt = 0.130030066;
            emo.disgust = 0.0107496157;
            emo.fear = 0.000155375863;
            emo.happiness = 0.08784503;
            emo.neutral = 0.72912854;
            emo.sadness = 0.0145995952;
            emo.surprise = 0.000662396953;

            Face face = new Face();
            face.height = 261;
            face.left = 140;
            face.top = 230;
            face.width = 261;

            EmoList emoList = new EmoList();

            EmotionChart emoChart = new EmotionChart();
            emoChart.scores = emo;
            emoChart.faceRectangle = face;

            emoList.Add(emoChart);

            JavaScriptSerializer jss = new JavaScriptSerializer();
            string s = jss.Serialize(emoList);

            return s;
        }

    }

}
