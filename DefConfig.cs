using OneQuick.Config;
using OneQuick.Core.Operations;
using OneQuick.Core.Triggers;
using System.Collections.Generic;

namespace OneQuick
{
    internal static class DefConfig
    {
        public static QuickSearchEntry NewQuickSearchEntry()
        {
            return new QuickSearchEntry("<New>", K.None, "%s", K.None);
        }

        public static ReplacePhrase NewReplacePhrase()
        {
            return new ReplacePhrase("input", "output")
            {
                Enable = true
            };
        }

        public static HotkeyRemap NewHotkeyRemap()
        {
            return new HotkeyRemap
            {
                Enable = true,
                Hotkey = new HotkeyTrigger(),
                Operation = new SendKey()
            };
        }

        public static KeyMappingItem NewKeyMappingItem()
        {
            return new KeyMappingItem
            {
                Enable = false
            };
        }

        public static Dictionary<string, ConfigFileEntry> BuildinConfigEntrys => new Dictionary<string, ConfigFileEntry>
                {
                    {
                        "Default",
                        new ConfigFileEntry("Default", "<Default>", "<Buildin>")
                        {
                            OverwriteScreenBorder = true,
                            OverwriteQuickSearch = true
                        }
                    },
                    {
                        "Empty",
                        new ConfigFileEntry("Empty", "<Empty>", "<Buildin>")
                    },
                    {
                        "Dvorak",
                        new ConfigFileEntry("Dvorak", "<Dvorak Keyboard>", "<Buildin>")
                        {
                            OverwriteKeyMapping = true
                        }
                    }
                };

        public static Dictionary<string, Configuration> BuildinConfigObjects => new Dictionary<string, Configuration>
                {
                    {
                        "Default",
                        ProdVersion
                    },
                    {
                        "Empty",
                        EmptyConfig
                    },
                    {
                        "Dvorak",
                        DvorakConfig
                    }
                };

        public static Configuration EmptyConfig => new Configuration
        {
            ScreenBorders = new ScreenBorderPackage(),
            QuickSearch = new ObservableCollectionX<QuickSearchEntry>(),
            ReplacePhrases = new ObservableCollectionX<ReplacePhrase>(),
            CustomKeys = new ObservableCollectionX<HotkeyRemap>(),
            KeyMapping = new ObservableCollectionX<KeyMappingItem>(),
            Buildin = new BuildinFuncs(false)
        };

        public static Configuration DvorakConfig => new Configuration
        {
            KeyMapping = new ObservableCollectionX<KeyMappingItem>
                    {
                        new KeyMappingItem(K.OemMinus, K.OemOpenBrackets),
                        new KeyMappingItem(K.Oemplus, K.OemCloseBrackets),
                        new KeyMappingItem(K.Q, K.OemQuotes),
                        new KeyMappingItem(K.W, K.Oemcomma),
                        new KeyMappingItem(K.E, K.OemPeriod),
                        new KeyMappingItem(K.R, K.P),
                        new KeyMappingItem(K.T, K.Y),
                        new KeyMappingItem(K.Y, K.F),
                        new KeyMappingItem(K.U, K.G),
                        new KeyMappingItem(K.I, K.C),
                        new KeyMappingItem(K.O, K.R),
                        new KeyMappingItem(K.P, K.L),
                        new KeyMappingItem(K.OemOpenBrackets, K.OemQuestion),
                        new KeyMappingItem(K.OemCloseBrackets, K.Oemplus),
                        new KeyMappingItem(K.S, K.O),
                        new KeyMappingItem(K.D, K.E),
                        new KeyMappingItem(K.F, K.U),
                        new KeyMappingItem(K.G, K.I),
                        new KeyMappingItem(K.H, K.D),
                        new KeyMappingItem(K.J, K.H),
                        new KeyMappingItem(K.K, K.T),
                        new KeyMappingItem(K.L, K.N),
                        new KeyMappingItem(K.OemSemicolon, K.S),
                        new KeyMappingItem(K.OemQuotes, K.OemMinus),
                        new KeyMappingItem(K.Z, K.OemSemicolon),
                        new KeyMappingItem(K.X, K.Q),
                        new KeyMappingItem(K.C, K.J),
                        new KeyMappingItem(K.V, K.K),
                        new KeyMappingItem(K.B, K.X),
                        new KeyMappingItem(K.N, K.B),
                        new KeyMappingItem(K.M, K.M),
                        new KeyMappingItem(K.Oemcomma, K.W),
                        new KeyMappingItem(K.OemPeriod, K.V),
                        new KeyMappingItem(K.OemQuestion, K.Z)
                    }
        };

        public static Configuration DebugVersion => new Configuration
        {
            ReplacePhrases = new ObservableCollectionX<ReplacePhrase>
                    {
                        new ReplacePhrase("001", "this is an test string"),
                        new ReplacePhrase(",./", "中文输出测试"),
                        new ReplacePhrase("   ", "space space space")
                    },
            ScreenBorders = new ScreenBorderPackage
            {
                LT = new ScreenBorder
                {
                    Wheel = WheelOperation.Volume,
                    NonMove = new RunCmd("notepad")
                }
            },
            CustomKeys = new ObservableCollectionX<HotkeyRemap>
                    {
                        new HotkeyRemap((K)393287, new RunCmd("https://google.com/ncr"))
                    },
            Buildin = new BuildinFuncs(true)
            {
                ShowHideWindow = new Kwrapper(K.F9),
                RunStopServer = new Kwrapper(K.F10),
                WindowInfo = new Kwrapper((K)589897),
                ExplorerAppPath = new Kwrapper((K)589893)
            }
        };

        public static Configuration ProdVersion => new Configuration
        {
            ScreenBorders = new ScreenBorderPackage
            {
                LT = new ScreenBorder
                {
                    Wheel = WheelOperation.Volume,
                    WheelClick = new SendKey(K.VolumeMute)
                },
                T = new ScreenBorder
                {
                    Wheel = WheelOperation.Tab
                },
                RT = new ScreenBorder
                {
                    Wheel = WheelOperation.Media,
                    WheelClick = new SendKey(K.MediaPlayPause)
                },
                L = new ScreenBorder
                {
                    Wheel = WheelOperation.PageDownUp
                },
                R = new ScreenBorder
                {
                    Wheel = WheelOperation.PageDownUp
                },
                B = new ScreenBorder
                {
                    Wheel = WheelOperation.VirtualDesktop
                }
            },
            QuickSearch = new ObservableCollectionX<QuickSearchEntry>
                    {
                        new QuickSearchEntry("Google", K.G, "https://google.com/search?q=%s", K.D1),
                        new QuickSearchEntry("百度", K.B, "https://www.baidu.com/s?wd=%s", K.D1),
                        new QuickSearchEntry("微博", K.W, "http://s.weibo.com/weibo/%s", K.D2),
                        new QuickSearchEntry("知乎", K.Z, "http://www.zhihu.com/search?q=%s", K.D2),
                        new QuickSearchEntry("Bilibili", K.L, "http://www.bilibili.com/search?keyword=%s", K.D3),
                        new QuickSearchEntry("Youtube", K.Y, "https://www.youtube.com/results?search_query=%s", K.D3),
                        new QuickSearchEntry("豆瓣电影", K.M, "http://movie.douban.com/subject_search?search_text=%s", K.None),
                        new QuickSearchEntry("QR-Code", K.Q, "http://api.qrserver.com/v1/create-qr-code/?data=%s", K.None),
                        new QuickSearchEntry("<直接运行>", (K)131085, "%s", K.None),
                        new QuickSearchEntry("打开B站视频(av号)", (K)262220, "https://www.bilibili.com/video/%s/", K.None)
                    },
            ReplacePhrases = new ObservableCollectionX<ReplacePhrase>
                    {
                        new ReplacePhrase(",,g", "your-email-address@gmail.com")
                        {
                            Enable = true
                        },
                        new ReplacePhrase("input_some_chars", "Auto replace to string, 输出可以是中文")
                        {
                            Enable = false
                        },
                        new ReplacePhrase("输入不能是中文", "Input CANNOT be chars outside english keyboard, like Chinese.")
                        {
                            Enable = false
                        },
                        new ReplacePhrase("输入长度限制", "输入字串长度不要超过10个字符。（Input Length Limit: 10 chars.）")
                        {
                            Enable = false
                        }
                    },
            CustomKeys = new ObservableCollectionX<HotkeyRemap>
                    {
                        new HotkeyRemap((K)524366, new RunCmd("notepad"))
                        {
                            Enable = true
                        },
                        new HotkeyRemap((K)524372, new BuildinOperation(BuildinOperationEnum.ToggleTopmost))
                        {
                            Enable = false
                        },
                        new HotkeyRemap((K)528385, new BuildinOperation(BuildinOperationEnum.OpacityUp))
                        {
                            Enable = false
                        },
                        new HotkeyRemap((K)528384, new BuildinOperation(BuildinOperationEnum.OpacityDown))
                        {
                            Enable = false
                        }
                    },
            KeyMapping = new ObservableCollectionX<KeyMappingItem>
                    {
                        new KeyMappingItem
                        {
                            Enable = false,
                            Key = new Kwrapper(K.RControlKey),
                            Value = new Kwrapper(K.LWin)
                        }
                    },
            Buildin = new BuildinFuncs(true)
            {
                ShowHideWindow = new Kwrapper(K.F9),
                RunStopServer = new Kwrapper(K.F10),
                WindowInfo = new Kwrapper((K)589897),
                ExplorerAppPath = new Kwrapper((K)589893),
                ChromeScrollTab = true,
                CloseNotepad = true
            }
        };
    }
}
