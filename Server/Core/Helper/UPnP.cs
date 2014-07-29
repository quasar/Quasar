using System;
using System.IO;

namespace Core
{
    static class UPnP
    {
        public static byte[] UPnPCommandLine()
        {
            return Convert.FromBase64String("TVqQAAMAAAAEAAAA//8AALgAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAA4fug4AtAnNIbgBTM0hVGhp" +
                                            "cyBwcm9ncmFtIGNhbm5vdCBiZSBydW4gaW4gRE9TIG1vZGUuDQ0KJA" +
                                            "AAAAAAAABQRQAATAEDAJym11MAAAAAAAAAAOAAAgELAQsAAA4AAAAI" +
                                            "AAAAAAAA/isAAAAgAAAAQAAAAABAAAAgAAAAAgAABAAAAAAAAAAEAA" +
                                            "AAAAAAAACAAAAAAgAAAAAAAAMAQIUAABAAABAAAAAAEAAAEAAAAAAA" +
                                            "ABAAAAAAAAAAAAAAAKwrAABPAAAAAEAAADgFAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAGAAAAwAAAA0KwAAHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAACAAAAAAAAAAAAAAACCAAAE" +
                                            "gAAAAAAAAAAAAAAC50ZXh0AAAABAwAAAAgAAAADgAAAAIAAAAAAAAA" +
                                            "AAAAAAAAACAAAGAucnNyYwAAADgFAAAAQAAAAAYAAAAQAAAAAAAAAA" +
                                            "AAAAAAAABAAABALnJlbG9jAAAMAAAAAGAAAAACAAAAFgAAAAAAAAAA" +
                                            "AAAAAAAAQAAAQgAAAAAAAAAAAAAAAAAAAADgKwAAAAAAAEgAAAACAA" +
                                            "UAfCEAALgJAAADAAAAAQAABgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOZyAQAAcCgQAAAKcmMAAH" +
                                            "AoEAAACnLFAABwKBAAAApyJwEAcCgQAAAKcokBAHAoEAAACgIoAgAA" +
                                            "BioAABswBwA1AAAAAQAAEXMRAAAKCwdvEgAACgwCFpooEwAACgoIBn" +
                                            "KzAQBwBigDAAAGF3K7AQBwbxQAAAom3gMm3gAqAAAAARAAAAAAAAAx" +
                                            "MQADAQAAARswAwB3AAAAAgAAEXMVAAAKChQLctUBAHAMFg0GctcBAH" +
                                            "AfUG8WAAAKBm8XAAAKbxgAAAoLBm8ZAAAKBywgB28aAAAKDAhy9QEA" +
                                            "cG8bAAAKEwQIEQRvHAAAChMF3iLeGiYJF1gNCRsvAt6xCRsxCXLVAQ" +
                                            "BwEwXeCN4ActUBAHAqEQUqAAEQAAAAABAARFQAGgEAAAEeAigdAAAK" +
                                            "KkJTSkIBAAEAAAAAAAwAAAB2Mi4wLjUwNzI3AAAAAAUAbAAAAKgCAA" +
                                            "AjfgAAFAMAAIwDAAAjU3RyaW5ncwAAAACgBgAA/AEAACNVUwCcCAAA" +
                                            "EAAAACNHVUlEAAAArAgAAAwBAAAjQmxvYgAAAAAAAAACAAABRxUCAA" +
                                            "kAAAAA+iUzABYAAAEAAAAcAAAAAgAAAAQAAAACAAAAHQAAAA0AAAAC" +
                                            "AAAAAQAAAAMAAAAAAAoAAQAAAAAABgA0AC0ABgB2AGQABgCNAGQABg" +
                                            "CqAGQABgDJAGQABgDiAGQABgD7AGQABgAWAWQABgAxAWQABgBpAUoB" +
                                            "BgB9AUoBBgCLAWQABgCkAWQABgDUAcEBOwDoAQAABgAXAvcBBgA3Av" +
                                            "cBBgBaAi0ACgCKAn8CCgCXAn8CCgCgAn8CBgDdAi0ACgDqAn8CCgAB" +
                                            "A38CDgAcAwkDDgAuAwkDDgBLA0ADBgB1Ay0AAAAAAAEAAAAAAAEAAQ" +
                                            "AAABAAEwAbAAUAAQABAFAgAAAAAJEAOwAKAAEAjCAAAAAAkQBAAAoA" +
                                            "AgDgIAAAAACRAEwAEAADAHQhAAAAAIYYWQAUAAMAAAABAF8AAAABAF" +
                                            "8AEQBZABgAGQBZABgAIQBZABgAKQBZABgAMQBZABgAOQBZABgAQQBZ" +
                                            "ABgASQBZABgAUQBZAB0AWQBZABgAYQBZABgAaQBZABgAcQBZACIAgQ" +
                                            "BZACgAiQBZABQAkQBiAi0AmQBZABQAoQC9AjIAsQDkAjcAqQD9AjwA" +
                                            "yQBZABQAyQAmA08AyQA1A1UA0QBUA1oAyQBmAxQACQBsA18A4QB8A2" +
                                            "MA4QCEA2gACQBZABQALgALAHgALgATAIgALgAbAIgALgAjAIgALgAr" +
                                            "AHgALgAzAI4ALgA7AIgALgBLAIgALgBTAKMALgBjAM0ALgBrANoALg" +
                                            "BzAOMALgB7AOwARwBtAASAAAABAAAAAAAAAAAAAAAAAFUCAAACAAAA" +
                                            "AAAAAAAAAAABACQAAAAAAAEAAAAAAAAAAAAAAAAAbAIAAAAAAgAAAA" +
                                            "AAAAAAAAAAAQAtAAAAAAAAAAAAADxNb2R1bGU+AHVwbnAuZXhlAFBy" +
                                            "b2dyYW0AeFJBVFVQblAAbXNjb3JsaWIAU3lzdGVtAE9iamVjdABNYW" +
                                            "luAEZvcndhcmRQb3J0AExvY2FsQWRkcmVzcwAuY3RvcgBhcmdzAFN5" +
                                            "c3RlbS5SZWZsZWN0aW9uAEFzc2VtYmx5VGl0bGVBdHRyaWJ1dGUAQX" +
                                            "NzZW1ibHlEZXNjcmlwdGlvbkF0dHJpYnV0ZQBBc3NlbWJseUNvbmZp" +
                                            "Z3VyYXRpb25BdHRyaWJ1dGUAQXNzZW1ibHlDb21wYW55QXR0cmlidX" +
                                            "RlAEFzc2VtYmx5UHJvZHVjdEF0dHJpYnV0ZQBBc3NlbWJseUNvcHly" +
                                            "aWdodEF0dHJpYnV0ZQBBc3NlbWJseVRyYWRlbWFya0F0dHJpYnV0ZQ" +
                                            "BBc3NlbWJseUN1bHR1cmVBdHRyaWJ1dGUAU3lzdGVtLlJ1bnRpbWUu" +
                                            "SW50ZXJvcFNlcnZpY2VzAENvbVZpc2libGVBdHRyaWJ1dGUAR3VpZE" +
                                            "F0dHJpYnV0ZQBBc3NlbWJseVZlcnNpb25BdHRyaWJ1dGUAQXNzZW1i" +
                                            "bHlGaWxlVmVyc2lvbkF0dHJpYnV0ZQBTeXN0ZW0uRGlhZ25vc3RpY3" +
                                            "MARGVidWdnYWJsZUF0dHJpYnV0ZQBEZWJ1Z2dpbmdNb2RlcwBTeXN0" +
                                            "ZW0uUnVudGltZS5Db21waWxlclNlcnZpY2VzAENvbXBpbGF0aW9uUm" +
                                            "VsYXhhdGlvbnNBdHRyaWJ1dGUAUnVudGltZUNvbXBhdGliaWxpdHlB" +
                                            "dHRyaWJ1dGUAdXBucABDb25zb2xlAFdyaXRlTGluZQBJbnRlcm9wLk" +
                                            "5BVFVQTlBMaWIATkFUVVBOUExpYgBVUG5QTkFUQ2xhc3MASVVQblBO" +
                                            "QVQASVN0YXRpY1BvcnRNYXBwaW5nQ29sbGVjdGlvbgBnZXRfU3RhdG" +
                                            "ljUG9ydE1hcHBpbmdDb2xsZWN0aW9uAFVJbnQxNgBQYXJzZQBJU3Rh" +
                                            "dGljUG9ydE1hcHBpbmcAQWRkAFVQblBOQVQAU3lzdGVtLk5ldC5Tb2" +
                                            "NrZXRzAFRjcENsaWVudABDb25uZWN0AFNvY2tldABnZXRfQ2xpZW50" +
                                            "AFN5c3RlbS5OZXQARW5kUG9pbnQAZ2V0X0xvY2FsRW5kUG9pbnQAQ2" +
                                            "xvc2UAVG9TdHJpbmcAU3RyaW5nAEluZGV4T2YAUmVtb3ZlAAAAYWUA" +
                                            "RgBKAEIAVgBDAFUAeQBNAEYAVgBRAGIAbABBAGwATQBqAEIATgBiAD" +
                                            "IAUgAxAGIARwBVAGwATQBqAEIAagBiADIAUgBsAFoAQwBVAHkATQBH" +
                                            "AEoANQBKAFQASQB3AABhWgBEAE4AaABaADIAeABsAEoAVABCAEIAZQ" +
                                            "BGAEoAQgBWAEMAVQB5AE0AQwBVAHkATQB6AEUAbABNAGoAQgBHAGMA" +
                                            "bQBWAGwASgBUAEkAdwBUADMAQgBsAGIAaQBVAHkAAGFNAEYATgB2AG" +
                                            "QAWABKAGoAWgBTAFUAeQBNAEYASgBsAGIAVwA5ADAAWgBTAFUAeQBN" +
                                            "AEUARgBrAGIAVwBsAHUASgBUAEkAdwBWAEcAOQB2AGIAQwBVAHkATQ" +
                                            "BHAE4AdgAAYVoARwBWAGsASgBUAEkAdwBZAG4AawBsAE0AagBCAE4A" +
                                            "WQBYAGgAWQBNAEgASQBsAE0AagBCAGgAYgBtAFEAbABNAGoAQgB2AG" +
                                            "MARwBWAHUATABYAE4AdgBkAFgASgBqAAApWgBTAFUAeQBNAEcATgB2" +
                                            "AGIAVwAxADEAYgBtAGwAMABlAFEAPQA9AAAHVABDAFAAABl4AFIAQQ" +
                                            "BUACAAMgAuADAALgAwAC4AMAAAAQAddwB3AHcALgBnAG8AbwBnAGwA" +
                                            "ZQAuAGMAbwBtAAADOgAAAAAA6l7cMIojEkK8oEZS/JCx4wAIt3pcVh" +
                                            "k04IkFAAEBHQ4DAAAOAyAAAQQgAQEOBCABAQIFIAEBET0EIAEBCAQA" +
                                            "AQEOBCAAElUEAAEHDgogBhJdCA4IDgIOBwcDBxJhElUFIAIBDggEIA" +
                                            "ASaQQgABJtAyAADgQgAQgOBCABDggKBwYSZRJtDggIDg8BAApVUG5Q" +
                                            "TW9kdWxlAAAFAQAAAAAUAQAPQ29kZWQgYnkgZDNhZ2xlAAApAQAkNW" +
                                            "I0MWVkMDItODM1MC00OWY0LTk2MmEtODE2YzUxMjNjNGY2AAAMAQAH" +
                                            "MS4wLjAuMAAACAEAAgAAAAAACAEACAAAAAAAHgEAAQBUAhZXcmFwTm" +
                                            "9uRXhjZXB0aW9uVGhyb3dzAQAAAAAAnKbXUwAAAAACAAAAXAAAAFAr" +
                                            "AABQDQAAUlNEU+dyWOXQd9JOkajeGWrAwJsJAAAAQzpcVXNlcnNcYm" +
                                            "Vhc3RcRGVza3RvcFxkM2FnbGV1cG5wXFVwbnBTdGF0XG9ialx4ODZc" +
                                            "UmVsZWFzZVx1cG5wLnBkYgDUKwAAAAAAAAAAAADuKwAAACAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAA4CsAAAAAAAAAAAAAAABfQ29yRXhlTWFp" +
                                            "bgBtc2NvcmVlLmRsbAAAAAAA/yUAIEAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAgAQAAAAIAAAgBgAAAA4AACAAAAA" +
                                            "AAAAAAAAAAAAAAABAAEAAABQAACAAAAAAAAAAAAAAAAAAAABAAEAAA" +
                                            "BoAACAAAAAAAAAAAAAAAAAAAABAAAAAACAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAABAAAAAACQAAAAoEAAAKQCAAAAAAAAAAAAAEhDAADqAQAAAAAAAA" +
                                            "AAAACkAjQAAABWAFMAXwBWAEUAUgBTAEkATwBOAF8ASQBOAEYATwAA" +
                                            "AAAAvQTv/gAAAQAAAAEAAAAAAAAAAQAAAAAAPwAAAAAAAAAEAAAAAQ" +
                                            "AAAAAAAAAAAAAAAAAAAEQAAAABAFYAYQByAEYAaQBsAGUASQBuAGYA" +
                                            "bwAAAAAAJAAEAAAAVAByAGEAbgBzAGwAYQB0AGkAbwBuAAAAAAAAAL" +
                                            "AEBAIAAAEAUwB0AHIAaQBuAGcARgBpAGwAZQBJAG4AZgBvAAAA4AEA" +
                                            "AAEAMAAwADAAMAAwADQAYgAwAAAAQAALAAEARgBpAGwAZQBEAGUAcw" +
                                            "BjAHIAaQBwAHQAaQBvAG4AAAAAAFUAUABuAFAATQBvAGQAdQBsAGUA" +
                                            "AAAAADAACAABAEYAaQBsAGUAVgBlAHIAcwBpAG8AbgAAAAAAMQAuAD" +
                                            "AALgAwAC4AMAAAADQACQABAEkAbgB0AGUAcgBuAGEAbABOAGEAbQBl" +
                                            "AAAAdQBwAG4AcAAuAGUAeABlAAAAAABEABAAAQBMAGUAZwBhAGwAQw" +
                                            "BvAHAAeQByAGkAZwBoAHQAAABDAG8AZABlAGQAIABiAHkAIABkADMA" +
                                            "YQBnAGwAZQAAADwACQABAE8AcgBpAGcAaQBuAGEAbABGAGkAbABlAG" +
                                            "4AYQBtAGUAAAB1AHAAbgBwAC4AZQB4AGUAAAAAADgACwABAFAAcgBv" +
                                            "AGQAdQBjAHQATgBhAG0AZQAAAAAAVQBQAG4AUABNAG8AZAB1AGwAZQ" +
                                            "AAAAAANAAIAAEAUAByAG8AZAB1AGMAdABWAGUAcgBzAGkAbwBuAAAA" +
                                            "MQAuADAALgAwAC4AMAAAADgACAABAEEAcwBzAGUAbQBiAGwAeQAgAF" +
                                            "YAZQByAHMAaQBvAG4AAAAxAC4AMAAuADAALgAwAAAAAAAAAO+7vzw/" +
                                            "eG1sIHZlcnNpb249IjEuMCIgZW5jb2Rpbmc9IlVURi04IiBzdGFuZG" +
                                            "Fsb25lPSJ5ZXMiPz4NCjxhc3NlbWJseSB4bWxucz0idXJuOnNjaGVt" +
                                            "YXMtbWljcm9zb2Z0LWNvbTphc20udjEiIG1hbmlmZXN0VmVyc2lvbj" +
                                            "0iMS4wIj4NCiAgPGFzc2VtYmx5SWRlbnRpdHkgdmVyc2lvbj0iMS4w" +
                                            "LjAuMCIgbmFtZT0iTXlBcHBsaWNhdGlvbi5hcHAiLz4NCiAgPHRydX" +
                                            "N0SW5mbyB4bWxucz0idXJuOnNjaGVtYXMtbWljcm9zb2Z0LWNvbTph" +
                                            "c20udjIiPg0KICAgIDxzZWN1cml0eT4NCiAgICAgIDxyZXF1ZXN0ZW" +
                                            "RQcml2aWxlZ2VzIHhtbG5zPSJ1cm46c2NoZW1hcy1taWNyb3NvZnQt" +
                                            "Y29tOmFzbS52MyI+DQogICAgICAgIDxyZXF1ZXN0ZWRFeGVjdXRpb2" +
                                            "5MZXZlbCBsZXZlbD0iYXNJbnZva2VyIiB1aUFjY2Vzcz0iZmFsc2Ui" +
                                            "Lz4NCiAgICAgIDwvcmVxdWVzdGVkUHJpdmlsZWdlcz4NCiAgICA8L3" +
                                            "NlY3VyaXR5Pg0KICA8L3RydXN0SW5mbz4NCjwvYXNzZW1ibHk+DQoA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAgAAAMAAAAADwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        }

        public static byte[] InteropNATUPNPLib_DLL()
        {
            return Convert.FromBase64String("TVqQAAMAAAAEAAAA//8AALgAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAA4fug4AtAnNIbgBTM0hVGhp" +
                                            "cyBwcm9ncmFtIGNhbm5vdCBiZSBydW4gaW4gRE9TIG1vZGUuDQ0KJA" +
                                            "AAAAAAAABQRQAATAEDABGg11MAAAAAAAAAAOAAAiELAQgAABQAAAAG" +
                                            "AAAAAAAAnjMAAAAgAAAAQAAAAABAAAAgAAAAAgAABAAAAAAAAAAEAA" +
                                            "AAAAAAAACAAAAAAgAAAAAAAAMAQIUAABAAABAAAAAAEAAAEAAAAAAA" +
                                            "ABAAAAAAAAAAAAAAAFAzAABLAAAAAEAAAIADAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAGAAAAwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAACAAAAAAAAAAAAAAACCAAAE" +
                                            "gAAAAAAAAAAAAAAC50ZXh0AAAApBMAAAAgAAAAFAAAAAIAAAAAAAAA" +
                                            "AAAAAAAAACAAAGAucnNyYwAAAIADAAAAQAAAAAQAAAAWAAAAAAAAAA" +
                                            "AAAAAAAABAAABALnJlbG9jAAAMAAAAAGAAAAACAAAAGgAAAAAAAAAA" +
                                            "AAAAAAAAQAAAQgAAAAAAAAAAAAAAAAAAAACAMwAAAAAAAEgAAAACAA" +
                                            "UAUCAAAAATAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEJTSkIBAAEAAAAAAAwAAA" +
                                            "B2Mi4wLjUwNzI3AAAAAAUAbAAAALAJAAAjfgAAHAoAABwFAAAjU3Ry" +
                                            "aW5ncwAAAAA4DwAACAAAACNVUwBADwAAEAAAACNHVUlEAAAAUA8AAL" +
                                            "ADAAAjQmxvYgAAAAAAAAACAAAQRzegAwkAAAAA+gEzABYAAAEAAAAN" +
                                            "AAAACQAAACwAAAA4AAAABQAAAAkAAABfAAAAKAAAAAcAAAAcAAAAHA" +
                                            "AAAAMAAAABAAAAAQAAAAAADQUBAAAAAAAGAK8AtgAGAL0AywAGAOoA" +
                                            "ywAGAP8AtgAGAAQBywAGABUBywAGAC0BywAGAD0BSQEGAFwBSQEGAG" +
                                            "gBywAGAH0BlAEGAKYBywAGAMMBywAAAAAAAQAAAAAAAQABAAEQAAAK" +
                                            "ABcABQABAAEAoRAAACIAFwAAAAEABQChEAAAKgAXAAAAAQAFAKEQAA" +
                                            "AzABcAAAABAAgAoRAAAFAAFwAAAAEADQChEAAAYwAXAAAAAQAYAKEQ" +
                                            "AACBABcAAAABAB0AoRAAAJUAFwAAAAEAKwAAAAAAAxAGGNsBCgABAA" +
                                            "AAAAADEMYJ4QEOAAEAAAAAAAMQxgkBAhMAAgAAAAAAAxDGCSICGAAD" +
                                            "AAAAAAADEMYN4QEOAAQAAAAAAAMQxg0BAhMABQAAAAAAAxDGDSICGA" +
                                            "AGAAAAAAADEMYFfwIsAAcAAAAAAAMQxg2NAjEACAAAAAAAAxDGDbEC" +
                                            "OAALAAAAAAADEMYFuwI8AAsAAAAAAAMQxgXCAkIADQAAAAAAAxDGDQ" +
                                            "sDWAAUAAAAAAADEMYNIQM4ABUAAAAAAAMQxg0yAzgAFQAAAAAAAxDG" +
                                            "DUMDWAAVAAAAAAADEMYNUANYABYAAAAAAAMQxg1jA1wAFwAAAAAAAx" +
                                            "DGDW8DWAAXAAAAAAADEMYFfwNgABgAAAAAAAMQxgWSA2UAGQAAAAAA" +
                                            "AxDGBZwDYAAaAAAAAAADEMYFrANqABsAAAAAAAMQxgV/AiwAHAAAAA" +
                                            "AAAxDGDY0CdwAdAAAAAAADEMYNsQI4ACEAAAAAAAMQxgW7An8AIQAA" +
                                            "AAAAAxDGBcIChgAkAAAAAAADEMYNCwNYAC0AAAAAAAMQxg0zBFgALg" +
                                            "AAAAAAAxDGDSEDOAAvAAAAAAADEMYNQwNYAC8AAAAAAAMQxg0yAzgA" +
                                            "MAAAAAAAAxDGDVADWAAwAAAAAAADEMYNYwNcADEAAAAAAAMQxg1vA1" +
                                            "gAMQAAAAAAAxDGDUIEOAAyAAAAAAADEMYFVASbADIAAAAAAAMQxgV/" +
                                            "A2AAMwAAAAAAAxDGBZIDZQA0AAAAAAADEMYFnANgADUAAAAAAAMQxg" +
                                            "WsA2oANgAAAAAAAxDGDY4EoAA3AAAAAAADEMYNrASgADgAACAAAAAA" +
                                            "ACAAAAAAACAAAAAAACAAAAAAACAAAAAAACAAAAAAACAAAAAAACAAAA" +
                                            "AAAQABAJYCASACAKQCAQABAJYCASACAKQCACAAAAAAAQABAJYCASAC" +
                                            "AKQCAQADAMYCASAEANQCAQAFAOcCASAGAPACACAAAAAAACAAAAAAAC" +
                                            "AAAAAAACAAAAAAASABANQCAQABAJkDASABAPACAQABAMYCACAAAAAA" +
                                            "ACAAAAAAASABABUEAQACAJYCASADAKQCASABABUEAQACAJYCASADAK" +
                                            "QCACAAAAAAASABABUEAQACAJYCASADAKQCAQAEAMYCASAFANQCAQAG" +
                                            "AOcCASAHAPACAQAIACQEACAAAAAAACAAAAAAACAAAAAAACAAAAAAAC" +
                                            "AAAAAAAQABAF8EASABANQCAQABAJkDASABAPACAQABAMYCASABAAAA" +
                                            "ASABAAAAAgAQAAIADAADABAABQAhAAcAIQARANsBYAAZANsBqQApAN" +
                                            "sBrgAxANsBqQA5ANsBagBRANsBqQBZANsBYABhANsBYABpANsBtAAp" +
                                            "ACsA7AEuAEsAoAMuAAsAZgMuAEMAkANAACsA7AFDACMA5QFDABMAlw" +
                                            "FDAAsAbQFJACsA9QFgACsA9QFjAAsAngFjABsAyAFpACsA/gGAACsA" +
                                            "/gGDABMABwKDAAsAngGJACsA7AGgACsA7AGjAAsADgKjADsAUQKjAB" +
                                            "MABwKpACsA9QHAACsA9QHDABMABwLDAAsAWwLJACsA/gHgACsA/gHj" +
                                            "ABMABwLjAAsAzQLjADsAUQLpACsASAIAASsAOAIAATMAQQIDAQsA9w" +
                                            "IDARMABwIJASsA7AEgASsASAIjARMABwIjAQsAPAMpASsA7AFAASsA" +
                                            "7AFJASsA9QFgASsA9QFpASsA/gGAASsA/gGJASsAhQKgASsA7AGpAS" +
                                            "sAjgLAASsA9QHJASsAlwLgASsA/gHpASsAoAIAAisAhQIJAisASAIg" +
                                            "AisAjgIpAisA7AFAAisAlwJJAisA7AFgAisAoAJpAisA9QGAAisAqQ" +
                                            "KJAisA/gGgAisAsgKpAisAhQLAAisAuwLJAisAjgLgAisAxALpAisA" +
                                            "lwIAAysAOAIAAzMAQQIJAysAoAIgAysASAIpAysAqQJAAysA7AFJAy" +
                                            "sAsgJgAysA9QFpAysA7AGAAysA/gGJAysA9QGgAysA7AHAAysA9QHg" +
                                            "AysA/gEABCsAhQIgBCsAjgJABCsAlwJgBCsAoAKABCsAqQKgBCsAsg" +
                                            "LABCsAuwLgBCsAxAIABSsAIQMgBSsAKgNABSsAMwNgBSsA7AGABSsA" +
                                            "9QEDALoABQC6AAcAugAJALoACwC6AA0AugAPALwAEQC6ABUAaQEZAG" +
                                            "kBGwC6AB8AaQEjAGkBJwBpASkAaQErAGkBLQBpAS8AaQExAGkBNQBp" +
                                            "ATkAvAA7ALoAPQBpAUEAaQFDAGkBRwBpAUkAugBLAGkBTwBpAVMAaQ" +
                                            "FXAGkBWwBpAV0AaQFfAGkBYQBpAWMAaQFnAGkBawBpAW8AawFxAGsB" +
                                            "AgABAAQABAAFAAcABgAJAAcAEAAIABIACQAbAAAANgIdAAAAUgIiAA" +
                                            "AAbwInAAAANgIdAAAAUgIiAAAAbwInAAAAAANNAAAABQNUAAAAvQNv" +
                                            "AAAAzwNUAAAA3ANUAAAA6QNvAAAA8gNvAAAAAQRzAAAACQRvAAAAAA" +
                                            "OTAAAABQNUAAAAvQNvAAAAdQRvAAAAzwNUAAAA6QNvAAAA3ANUAAAA" +
                                            "8gNvAAAAAQRzAAAACQRvAAAAgARUAAAAyASlAAAA4gSlAAIAAgADAA" +
                                            "IAAwAFAAIABAAHAAIABQAJAAIABgALAAIABwANAAIACQAPAAIACgAR" +
                                            "AAIADQATAAIADgAVAAIADwAXAAIAEAAZAAIAEQAbAAIAEgAdAAIAEw" +
                                            "AfAAIAGQAhAAIAGgAjAAIAHQAlAAIAHgAnAAIAHwApAAIAIAArAAIA" +
                                            "IQAtAAIAIgAvAAIAIwAxAAIAJAAzAAIAJQA1AAEAKwA3AAEALAA5AA" +
                                            "IABAAKAAIABgAMAAIACAAOAASAAAABAAAAAAAAAAAAAAAAAPoEAAAC" +
                                            "AAAAAAAAAAAAAAABAKYAAAAAAAAAADxNb2R1bGU+AFVQblBOQVRDbG" +
                                            "FzcwBOQVRVUE5QTGliAFVQblBOQVQASVVQblBOQVQASVN0YXRpY1Bv" +
                                            "cnRNYXBwaW5nQ29sbGVjdGlvbgBJU3RhdGljUG9ydE1hcHBpbmcASU" +
                                            "R5bmFtaWNQb3J0TWFwcGluZ0NvbGxlY3Rpb24ASUR5bmFtaWNQb3J0" +
                                            "TWFwcGluZwBJTkFURXZlbnRNYW5hZ2VyAG1zY29ybGliAE9iamVjdA" +
                                            "BTeXN0ZW0AR3VpZEF0dHJpYnV0ZQBTeXN0ZW0uUnVudGltZS5JbnRl" +
                                            "cm9wU2VydmljZXMAVHlwZUxpYlR5cGVBdHRyaWJ1dGUAVHlwZQBDb0" +
                                            "NsYXNzQXR0cmlidXRlAENsYXNzSW50ZXJmYWNlQXR0cmlidXRlAERp" +
                                            "c3BJZEF0dHJpYnV0ZQBJRW51bWVyYWJsZQBTeXN0ZW0uQ29sbGVjdG" +
                                            "lvbnMASUVudW1lcmF0b3IAVHlwZUxpYkZ1bmNBdHRyaWJ1dGUARGVm" +
                                            "YXVsdE1lbWJlckF0dHJpYnV0ZQBTeXN0ZW0uUmVmbGVjdGlvbgBJbX" +
                                            "BvcnRlZEZyb21UeXBlTGliQXR0cmlidXRlAFR5cGVMaWJWZXJzaW9u" +
                                            "QXR0cmlidXRlAC5jdG9yAGdldF9TdGF0aWNQb3J0TWFwcGluZ0NvbG" +
                                            "xlY3Rpb24AZ2V0X0R5bmFtaWNQb3J0TWFwcGluZ0NvbGxlY3Rpb24A" +
                                            "Z2V0X05BVEV2ZW50TWFuYWdlcgBTdGF0aWNQb3J0TWFwcGluZ0NvbG" +
                                            "xlY3Rpb24ARHluYW1pY1BvcnRNYXBwaW5nQ29sbGVjdGlvbgBOQVRF" +
                                            "dmVudE1hbmFnZXIAR2V0RW51bWVyYXRvcgBnZXRfSXRlbQBsRXh0ZX" +
                                            "JuYWxQb3J0AGJzdHJQcm90b2NvbABnZXRfQ291bnQAUmVtb3ZlAEFk" +
                                            "ZABsSW50ZXJuYWxQb3J0AGJzdHJJbnRlcm5hbENsaWVudABiRW5hYm" +
                                            "xlZABic3RyRGVzY3JpcHRpb24ASXRlbQBDb3VudABnZXRfRXh0ZXJu" +
                                            "YWxJUEFkZHJlc3MAZ2V0X0V4dGVybmFsUG9ydABnZXRfSW50ZXJuYW" +
                                            "xQb3J0AGdldF9Qcm90b2NvbABnZXRfSW50ZXJuYWxDbGllbnQAZ2V0" +
                                            "X0VuYWJsZWQAZ2V0X0Rlc2NyaXB0aW9uAEVkaXRJbnRlcm5hbENsaW" +
                                            "VudABFbmFibGUAdmIARWRpdERlc2NyaXB0aW9uAEVkaXRJbnRlcm5h" +
                                            "bFBvcnQARXh0ZXJuYWxJUEFkZHJlc3MARXh0ZXJuYWxQb3J0AEludG" +
                                            "VybmFsUG9ydABQcm90b2NvbABJbnRlcm5hbENsaWVudABFbmFibGVk" +
                                            "AERlc2NyaXB0aW9uAGJzdHJSZW1vdGVIb3N0AGxMZWFzZUR1cmF0aW" +
                                            "9uAGdldF9SZW1vdGVIb3N0AGdldF9MZWFzZUR1cmF0aW9uAFJlbmV3" +
                                            "TGVhc2UAbExlYXNlRHVyYXRpb25EZXNpcmVkAFJlbW90ZUhvc3QATG" +
                                            "Vhc2VEdXJhdGlvbgBzZXRfRXh0ZXJuYWxJUEFkZHJlc3NDYWxsYmFj" +
                                            "awBzZXRfTnVtYmVyT2ZFbnRyaWVzQ2FsbGJhY2sARXh0ZXJuYWxJUE" +
                                            "FkZHJlc3NDYWxsYmFjawBOdW1iZXJPZkVudHJpZXNDYWxsYmFjawBJ" +
                                            "bnRlcm9wLk5BVFVQTlBMaWIATkFUVVBOUExpYi5kbGwAAAMgAAAAAA" +
                                            "ArOPYTiy5WRZ07Rkj6yj2kAAi3elxWGTTgiQMgAAEEIAASFAQgABIc" +
                                            "BCAAEiQECAASFAQIABIcBAgAEiQEIAASJQYgAhIYCA4DIAAIBSACAQ" +
                                            "gOCiAGEhgIDggOAg4GCAISGAgOAwgACAMgAA4DIAACBCABAQ4EIAEB" +
                                            "AgQgAQEIAwgADgMIAAIHIAMSIA4IDgYgAwEOCA4MIAgSIA4IDggOAg" +
                                            "4IBwgDEiAOCA4EIAEICAQgAQEcAwgAHAQgAQEGBSABARIRBSACAQgI" +
                                            "ARyAqywAAIClU3lzdGVtLlJ1bnRpbWUuSW50ZXJvcFNlcnZpY2VzLk" +
                                            "N1c3RvbU1hcnNoYWxlcnMuRW51bWVyYXRvclRvRW51bVZhcmlhbnRN" +
                                            "YXJzaGFsZXIsIEN1c3RvbU1hcnNoYWxlcnMsIFZlcnNpb249Mi4wLj" +
                                            "AuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1iMDNm" +
                                            "NWY3ZjExZDUwYTNhAAETARkpAQAkQUUxRTAwQUEtM0ZENS00MDNDLT" +
                                            "hBMjctMkJCREMzMENEMEUxAAAGAQACAAAAKQEAJEIxNzFDODEyLUND" +
                                            "NzYtNDg1QS05NEQ4LUI2QjNBMjc5NEU5OQAAHAEAF05BVFVQTlBMaW" +
                                            "IuVVBuUE5BVENsYXNzAAAGAQAAAAAACAEAAQAAAAAACAEAAgAAAAAA" +
                                            "CAEAAwAAAAAABgEAQBAAACkBACRDRDFGM0U3Ny02NkQ2LTQ2NjQtOD" +
                                            "JDNy0zNkRCQjY0MUQwRjEAAAgBAPz///8AAAYBAEEAAAAIAQAAAAAA" +
                                            "AAAJAQAESXRlbQAAKQEAJDZGMTA3MTFGLTcyOUItNDFFNS05M0I4LU" +
                                            "YyMUQwRjgxOERGMQAACAEABAAAAAAACAEABQAAAAAACAEABgAAAAAA" +
                                            "CAEABwAAAAAACAEACAAAAAAACAEACQAAAAAACAEACgAAAAAACAEACw" +
                                            "AAAAAAKQEAJEI2MERFMDBGLTE1NkUtNEU4RC05RUMxLTNBMjM0MkMx" +
                                            "MDg5OQAAKQEAJDRGQzgwMjgyLTIzQjYtNDM3OC05QTI3LUNEOEYxN0" +
                                            "M5NDAwQwAACAEADAAAAAAACAEADQAAAAAACAEADgAAAAAAKQEAJDYy" +
                                            "NEJENTg4LTkwNjAtNDEwOS1CMEIwLTFBREJCQ0FDMzJERgAAKQEAJD" +
                                            "FjNTY1ODU4LWYzMDItNDcxZS1iNDA5LWYxODBhYTRhYmVjNgAADwEA" +
                                            "Ck5BVFVQTlBMaWIAAAwBAAEAAAAAAAAAAAAAAAB4MwAAAAAAAAAAAA" +
                                            "COMwAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgDMAAAAAAAAAAF9D" +
                                            "b3JEbGxNYWluAG1zY29yZWUuZGxsAAAAAAD/JQAgQAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAQAQAAAAGAAAgAAAAAAAAAAAAAAAAAAA" +
                                            "AQABAAAAMAAAgAAAAAAAAAAAAAAAAAAAAQAAAAAASAAAAFhAAAAkAw" +
                                            "AAAAAAAAAAAAAkAzQAAABWAFMAXwBWAEUAUgBTAEkATwBOAF8ASQBO" +
                                            "AEYATwAAAAAAvQTv/gAAAQAAAAEAAAAAAAAAAQAAAAAAPwAAAAAAAA" +
                                            "AEAAAAAgAAAAAAAAAAAAAAAAAAAEQAAAABAFYAYQByAEYAaQBsAGUA" +
                                            "SQBuAGYAbwAAAAAAJAAEAAAAVAByAGEAbgBzAGwAYQB0AGkAbwBuAA" +
                                            "AAAAB/ALAEhAIAAAEAUwB0AHIAaQBuAGcARgBpAGwAZQBJAG4AZgBv" +
                                            "AAAAYAIAAAEAMAAwADcAZgAwADQAYgAwAAAAHAACAAEAQwBvAG0AbQ" +
                                            "BlAG4AdABzAAAAIAAAACQAAgABAEMAbwBtAHAAYQBuAHkATgBhAG0A" +
                                            "ZQAAAAAAIAAAACwAAgABAEYAaQBsAGUARABlAHMAYwByAGkAcAB0AG" +
                                            "kAbwBuAAAAAAAgAAAAMAAIAAEARgBpAGwAZQBWAGUAcgBzAGkAbwBu" +
                                            "AAAAAAAxAC4AMAAuADAALgAwAAAASAATAAEASQBuAHQAZQByAG4AYQ" +
                                            "BsAE4AYQBtAGUAAABJAG4AdABlAHIAbwBwAC4ATgBBAFQAVQBQAE4A" +
                                            "UABMAGkAYgAAAAAAKAACAAEATABlAGcAYQBsAEMAbwBwAHkAcgBpAG" +
                                            "cAaAB0AAAAIAAAACwAAgABAEwAZQBnAGEAbABUAHIAYQBkAGUAbQBh" +
                                            "AHIAawBzAAAAAAAgAAAAWAAXAAEATwByAGkAZwBpAG4AYQBsAEYAaQ" +
                                            "BsAGUAbgBhAG0AZQAAAEkAbgB0AGUAcgBvAHAALgBOAEEAVABVAFAA" +
                                            "TgBQAEwAaQBiAC4AZABsAGwAAAAAAIQAMgABAFAAcgBvAGQAdQBjAH" +
                                            "QATgBhAG0AZQAAAAAAQQBzAHMAZQBtAGIAbAB5ACAAaQBtAHAAbwBy" +
                                            "AHQAZQBkACAAZgByAG8AbQAgAHQAeQBwAGUAIABsAGkAYgByAGEAcg" +
                                            "B5ACAAJwBOAEEAVABVAFAATgBQAEwAaQBiACcALgAAADQACAABAFAA" +
                                            "cgBvAGQAdQBjAHQAVgBlAHIAcwBpAG8AbgAAADEALgAwAC4AMAAuAD" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAMAAADAAAAKAzAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==");
        }

        public static string UPnP_Path()
        {
            return Path.GetTempPath() + "//tmpupnp.exe";
        }

        public static string UPnP_DLL_Path()
        {
            return Path.GetTempPath() + "//Interop.NATUPNPLib.dll";
        }

        public static void UseUPnP(ushort port)
        {
            if (!File.Exists(UPnP_Path()))
            {
                try
                {
                    File.WriteAllBytes(UPnP_Path(), UPnPCommandLine());
                }
                catch
                { }
            }

            if (!File.Exists(UPnP_DLL_Path()))
            {
                try
                {
                    File.WriteAllBytes(UPnP_DLL_Path(), InteropNATUPNPLib_DLL());
                }
                catch
                { }
            }

            if (File.Exists(UPnP_Path()) && File.Exists(UPnP_DLL_Path()))
            {
                try
                {
                    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(UPnP_Path(), port.ToString());
                    info.CreateNoWindow = true;
                    info.UseShellExecute = false;
                    System.Diagnostics.Process upnpProc = System.Diagnostics.Process.Start(info);
                }
                catch
                { }
            }
        }
    }
}
