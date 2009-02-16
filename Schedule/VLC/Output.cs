using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Nahravadlo.Schedule.VLC
{
    public enum Container
    {
        [NameValue("MPEG PS", "ps")]
        MpegPS,
        [NameValue("MPEG TS", "ts")]
        MpegTS,
        [NameValue("MPEG 1", "mpeg")]
        Mpeg1,
        [NameValue("OGM", "ogg")]
        Ogm,
        [NameValue("ASF", "asf")]
        Asf,
        [NameValue("MP4", "mp4")]
        Mp4,
        [NameValue("MOV", "mov")]
        Mov,
        [NameValue("Raw", "")]
        Raw,
        [NameValue("FLV", "flv")]
        Flv,
        [NameValue("MKV", "mkv")]
        Mkv
    }

    public enum VideoCodec
    {
        [NameValue("", "")]
        Same,
        [NameValue("MPEG-1", "mp1v")]
        Mpeg1,
        [NameValue("MPEG-2", "mp2v")]
        Mpeg2,
        [NameValue("MPEG-4", "mp4v")]
        Mpeg4,
        [NameValue("DivX 1", "div1")]
        DivX1,
        [NameValue("DivX 2", "div2")]
        DivX2,
        [NameValue("DivX 3", "div3")]
        DivX3,
        [NameValue("H263", "h263")]
        H263,
        [NameValue("H264", "h264")]
        H264,
        [NameValue("WMV 1", "wmv1")]
        Wmv1,
        [NameValue("WMV 2", "wmv2")]
        Wmv2,
        [NameValue("M-Jpeg", "mjpg")]
        MJpeg,
        [NameValue("Theora", "theo")]
        Theora
    }

    public enum AudioCodec
    {
        [NameValue("", "")]
        Same,
        [NameValue("MPEG Audio", "mpga")]
        MpegAudio,
        [NameValue("MP3", "mp3")]
        Mp3,
        [NameValue("MPEG 4 Audio", "mp4a")]
        Mpeg4Audio,
        [NameValue("A52/AC-3", "a52")]
        Ac3,
        [NameValue("Vorbis", "vorb")]
        Vorbis,
        [NameValue("Flac", "flac")]
        Flac,
        [NameValue("Speex", "spx")]
        Speex,
        [NameValue("Wav", "s16l")]
        Wav,
        [NameValue("WMA", "wma")]
        Wma
    }

    public class Output
    {
        public Output()
        {
            URI = null;
            FileName = null;
            Container = Container.Raw;
            AudioCodec = AudioCodec.Same;
            AudioBitrate = 128;
            AudioChannels = 2;
            VideoCodec = VideoCodec.Same;
            VideoBitrate = 800;
            PlayLocally = false;
        }

        public Output(string uri, string filename, Container container, AudioCodec audioCodec, byte audioChannels, int audioBitrate, VideoCodec videoCodec, int videoBitrate)
        {
            URI = uri;
            FileName = filename;
            Container = container;
            AudioCodec = audioCodec;
            AudioBitrate = audioBitrate;
            AudioChannels = audioChannels;
            VideoCodec = videoCodec;
            VideoBitrate = videoBitrate;
        }

        public Output(string uri, string fileName)
        {
            URI = uri;
            FileName = fileName;
        }

        public string URI { get; set; }
        public string FileName { get; set; }
        public Container Container { get; set; }
        public AudioCodec AudioCodec { get; set; }
        public byte AudioChannels { get; set; }
        public int AudioBitrate { get; set; }
        public VideoCodec VideoCodec { get; set; }
        public int VideoBitrate { get; set; }
        public bool PlayLocally { get; set; }

        /*
        public static Output Parse(string parameter)
        {
            var output = new Output();
            //TODO parsovani URI
            
            int position;

            if ((position = parameter.IndexOf(":sout=#")) > -1)
            {
                position += 7;
                while(parameter.Length > position && parameter[position]!='}')
                {
                    if (parameter[position] == ':')
                    {
                        position++;
                        continue;
                    }

                    var endPosition = parameter.IndexOf('{', position);
                    if (endPosition == -1)
                        break;
                    var section = parameter.Substring(position, endPosition - position);

                    position += section.Length + 1;
                    endPosition = EndSection(parameter, position);

                    var sectionBody = parameter.Substring(position + 1, endPosition - position - 2);

                    switch (section.ToLowerInvariant())
                    {
                        case "transcode":
                            foreach (var item in SectionSplit(sectionBody))
                            {
                                switch(item)
                                {
                                    
                                }
                            }
                            
                        case "duplicate":
                        default:
                            break;
                    }
                    position = endPosition + 1;
                }
            }

            return output;
        }

        private static int EndSection(string text, int start)
        {
            var position = start;
            var parenthesCount = 0;

            while (text[position] != '}' && parenthesCount == 0)
            {
                if (text[position] == '{')
                {
                    parenthesCount++;
                }
                else if (text[position] != '}')
                {
                    parenthesCount--;
                }

                position++;
            }

            return position;
        }

        private static KeyValuePair<string,string>[] SectionSplit(string body)
        {
            var quote = false;
            int parenthesCount = 0;
            var ret = new List<KeyValuePair<string, string>>();
            
            var sb = new StringBuilder();
            string name = null;

            var position = 0;

            while(position < body.Length)
            {
                if (body[position] == '{')
                    parenthesCount++;
                if (body[position] == '}')
                    parenthesCount--;
                if (body[position] == '"' && parenthesCount == 0)
                {
                    quote = !quote;
                    position++;
                    continue;
                }

                if (body[position] == '=' && !quote && name == null && parenthesCount == 0)
                {
                    name = sb.ToString();
                    sb = new StringBuilder();
                    position++;
                    continue;
                }
                if (body[position] == ',' && !quote && parenthesCount == 0)
                {
                    if (name == null)
                    {
                        ret.Add(new KeyValuePair<string, string>(sb.ToString(), null));
                    }
                    else
                    {
                        ret.Add(new KeyValuePair<string, string>(name, sb.ToString()));
                    }
                    sb = new StringBuilder();
                    position++;
                    name = null;
                    continue;
                }
                sb.Append(body[position]);
                position++;
                continue;
            }
            return ret.ToArray();
        }
        */

        public static Output Parse(string parameter)
        {
            var r = new Regex("(?<uri>((udp(stream)?|rtp):(//)?([0-9:@.]+)))?.*(:demuxdump-file=\"|:sout=#(transcode{(vcodec=(?<videocodec>([^,]+)),vb=(?<videobitrate>([^,]+)),scale=1)?,?(acodec=(?<audiocodec>([^,]+)),ab=(?<audiobitrate>([^,]+)),channels=(?<audiochannels>([^}]+)))?}:)?duplicate{(?<playlocally>dst=display,)?dst=std{access=file,mux=(?<container>([^,]+)),(url|dst)=\")(?<filename>([^\"]+))?(\"|\"}})");
            var m = r.Match(parameter);
            if (m == null)
                return null;

            var ret = new Output {URI = m.Groups["uri"].Value, FileName = m.Groups["filename"].Value};

            if (m.Groups["videocodec"] != null)
            {
                ret.VideoCodec = (VideoCodec) NameValueAttribute.FromValue(typeof(VideoCodec), m.Groups["videocodec"].Value);
                ret.VideoBitrate = int.Parse(m.Groups["videobitrate"].Value, CultureInfo.InvariantCulture);
            }
            if (m.Groups["audiocodec"] != null)
            {
                ret.AudioCodec = (AudioCodec)NameValueAttribute.FromValue(typeof(AudioCodec), m.Groups["audiocodec"].Value);
                ret.AudioBitrate = int.Parse(m.Groups["audiobitrate"].Value, CultureInfo.InvariantCulture);
                ret.AudioChannels = byte.Parse(m.Groups["audiochannels"].Value, CultureInfo.InvariantCulture);
            }

            if (m.Groups["container"] !=  null)
            {
                ret.Container = (Container)NameValueAttribute.FromValue(typeof(Container), m.Groups["container"].Value);
            }

            return ret;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(URI);
            sb.Append(" ");
            sb.Append(":sout=#");
            if (AudioCodec != AudioCodec.Same || VideoCodec != VideoCodec.Same)
            {
                sb.Append("transcode{");
                if (VideoCodec != VideoCodec.Same)
                {
                    sb.AppendFormat("vcodec={0},vb={1},scale=1", NameValueAttribute.FromEnum(VideoCodec).Value, VideoBitrate);
                    if (AudioCodec != AudioCodec.Same)
                        sb.Append(",");
                }
                if (AudioCodec != AudioCodec.Same)
                {
                    sb.AppendFormat("acodec={0},ab={1},channels={2}", NameValueAttribute.FromEnum(AudioCodec).Value,
                                    AudioBitrate, AudioChannels);
                }
                sb.Append("}:");
            }
            sb.Append("duplicate{");
            if (PlayLocally)
                sb.Append("dst=display,");
            sb.AppendFormat("dst=std{{access=udp,mux={0},dst=\"{1}\"}}", NameValueAttribute.FromEnum(Container).Value,
                            FileName);
            return sb.ToString();
        }

        public static void Mmain(string[] args)
        {
            var o = Parse("udp://@223.0.0.1 :sout=#transcode{vcodec=mp4v,vb=1024,scale=1,acodec=mpga,ab=192,channels=2}:duplicate{dst=std{access=file,mux=ts,dst=\"aaa.mpeg\"}}");
            MessageBox.Show(o.ToString());
        }
    }
}