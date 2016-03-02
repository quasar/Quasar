using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using xClient.Core.Data;

namespace xClient.Core.Helper
{
    public static class GeoLocationHelper
    {
        public static readonly string[] ImageList =
        {
            "ad.png", "ae.png", "af.png", "ag.png", "ai.png", "al.png",
            "am.png", "an.png", "ao.png", "ar.png", "as.png", "at.png", "au.png", "aw.png", "ax.png", "az.png", "ba.png",
            "bb.png", "bd.png", "be.png", "bf.png", "bg.png", "bh.png", "bi.png", "bj.png", "bm.png", "bn.png", "bo.png",
            "br.png", "bs.png", "bt.png", "bv.png", "bw.png", "by.png", "bz.png", "ca.png", "catalonia.png", "cc.png",
            "cd.png", "cf.png", "cg.png", "ch.png", "ci.png", "ck.png", "cl.png", "cm.png", "cn.png", "co.png", "cr.png",
            "cs.png", "cu.png", "cv.png", "cx.png", "cy.png", "cz.png", "de.png", "dj.png", "dk.png", "dm.png", "do.png",
            "dz.png", "ec.png", "ee.png", "eg.png", "eh.png", "england.png", "er.png", "es.png", "et.png",
            "europeanunion.png", "fam.png", "fi.png", "fj.png", "fk.png", "fm.png", "fo.png", "fr.png", "ga.png",
            "gb.png", "gd.png", "ge.png", "gf.png", "gh.png", "gi.png", "gl.png", "gm.png", "gn.png", "gp.png", "gq.png",
            "gr.png", "gs.png", "gt.png", "gu.png", "gw.png", "gy.png", "hk.png", "hm.png", "hn.png", "hr.png", "ht.png",
            "hu.png", "id.png", "ie.png", "il.png", "in.png", "io.png", "iq.png", "ir.png", "is.png", "it.png", "jm.png",
            "jo.png", "jp.png", "ke.png", "kg.png", "kh.png", "ki.png", "km.png", "kn.png", "kp.png", "kr.png", "kw.png",
            "ky.png", "kz.png", "la.png", "lb.png", "lc.png", "li.png", "lk.png", "lr.png", "ls.png", "lt.png", "lu.png",
            "lv.png", "ly.png", "ma.png", "mc.png", "md.png", "me.png", "mg.png", "mh.png", "mk.png", "ml.png", "mm.png",
            "mn.png", "mo.png", "mp.png", "mq.png", "mr.png", "ms.png", "mt.png", "mu.png", "mv.png", "mw.png", "mx.png",
            "my.png", "mz.png", "na.png", "nc.png", "ne.png", "nf.png", "ng.png", "ni.png", "nl.png", "no.png", "np.png",
            "nr.png", "nu.png", "nz.png", "om.png", "pa.png", "pe.png", "pf.png", "pg.png", "ph.png", "pk.png", "pl.png",
            "pm.png", "pn.png", "pr.png", "ps.png", "pt.png", "pw.png", "py.png", "qa.png", "re.png", "ro.png", "rs.png",
            "ru.png", "rw.png", "sa.png", "sb.png", "sc.png", "scotland.png", "sd.png", "se.png", "sg.png", "sh.png",
            "si.png", "sj.png", "sk.png", "sl.png", "sm.png", "sn.png", "so.png", "sr.png", "st.png", "sv.png", "sy.png",
            "sz.png", "tc.png", "td.png", "tf.png", "tg.png", "th.png", "tj.png", "tk.png", "tl.png", "tm.png", "tn.png",
            "to.png", "tr.png", "tt.png", "tv.png", "tw.png", "tz.png", "ua.png", "ug.png", "um.png", "us.png", "uy.png",
            "uz.png", "va.png", "vc.png", "ve.png", "vg.png", "vi.png", "vn.png", "vu.png", "wales.png", "wf.png",
            "ws.png", "ye.png", "yt.png", "za.png", "zm.png", "zw.png"
        };

        public static int ImageIndex { get; set; }
        public static GeoInformation GeoInfo { get; private set; }
        public static DateTime LastLocated { get; private set; }
        public static bool LocationCompleted { get; private set; }

        static GeoLocationHelper()
        {
            LastLocated = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        public static void Initialize()
        {
            TimeSpan lastLocateTry = new TimeSpan(DateTime.UtcNow.Ticks - LastLocated.Ticks);

            // last location was 30 minutes ago or last location has not completed
            if (lastLocateTry.TotalMinutes > 30 || !LocationCompleted)
            {
                TryLocate();

                if (GeoInfo.country_code == "-" || GeoInfo.country == "Unknown")
                {
                    ImageIndex = 247; // question icon
                    return;
                }

                for (int i = 0; i < ImageList.Length; i++)
                {
                    if (ImageList[i].Contains(GeoInfo.country_code.ToLower()))
                    {
                        ImageIndex = i;
                        break;
                    }
                }
            }
        }

        private static void TryLocate()
        {
            LocationCompleted = false;

            try
            {
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(GeoInformation));

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://telize.com/geoip");
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; rv:36.0) Gecko/20100101 Firefox/36.0";
                request.Proxy = null;
                request.Timeout = 10000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            string responseString = reader.ReadToEnd();

                            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(responseString)))
                            {
                                GeoInfo = (GeoInformation)jsonSerializer.ReadObject(ms);
                            }
                        }
                    }
                }

                LastLocated = DateTime.UtcNow;
                LocationCompleted = true;
            }
            catch
            {
                TryLocateFallback();
            }
        }

        private static void TryLocateFallback()
        {
            GeoInfo = new GeoInformation();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://freegeoip.net/xml/");
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; rv:36.0) Gecko/20100101 Firefox/36.0";
                request.Proxy = null;
                request.Timeout = 10000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            string responseString = reader.ReadToEnd();

                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(responseString);

                            string xmlIp = doc.SelectSingleNode("Response//IP").InnerXml;
                            string xmlCountry = doc.SelectSingleNode("Response//CountryName").InnerXml;
                            string xmlCountryCode = doc.SelectSingleNode("Response//CountryCode").InnerXml;
                            string xmlRegion = doc.SelectSingleNode("Response//RegionName").InnerXml;
                            string xmlCity = doc.SelectSingleNode("Response//City").InnerXml;
                            string timeZone = doc.SelectSingleNode("Response//TimeZone").InnerXml;

                            GeoInfo.ip = (!string.IsNullOrEmpty(xmlIp))
                                ? xmlIp
                                : "-";
                            GeoInfo.country = (!string.IsNullOrEmpty(xmlCountry))
                                ? xmlCountry
                                : "Unknown";
                            GeoInfo.country_code = (!string.IsNullOrEmpty(xmlCountryCode))
                                ? xmlCountryCode
                                : "-";
                            GeoInfo.region = (!string.IsNullOrEmpty(xmlRegion))
                                ? xmlRegion
                                : "Unknown";
                            GeoInfo.city = (!string.IsNullOrEmpty(xmlCity))
                                ? xmlCity
                                : "Unknown";
                            GeoInfo.timezone = (!string.IsNullOrEmpty(timeZone))
                                ? timeZone
                                : "Unknown";

                            GeoInfo.isp = "Unknown"; // freegeoip does not support ISP detection
                        }
                    }
                }

                LastLocated = DateTime.UtcNow;
                LocationCompleted = true;
            }
            catch
            {
                GeoInfo.country = "Unknown";
                GeoInfo.country_code = "-";
                GeoInfo.region = "Unknown";
                GeoInfo.city = "Unknown";
                GeoInfo.timezone = "Unknown";
                GeoInfo.isp = "Unknown";
                LocationCompleted = false;
            }

            if (string.IsNullOrEmpty(GeoInfo.ip))
                TryGetWanIp();
        }

        private static void TryGetWanIp()
        {
            string wanIp = "-";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://api.ipify.org/");
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; rv:36.0) Gecko/20100101 Firefox/36.0";
                request.Proxy = null;
                request.Timeout = 5000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            wanIp = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            GeoInfo.ip = wanIp;
        }
    }
}