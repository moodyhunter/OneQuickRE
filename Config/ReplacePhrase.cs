using OneQuick.Core.Operations;
using OneQuick.Core.Triggers;
using System.Collections.Generic;

namespace OneQuick.Config
{
    public class ReplacePhrase : ConfigEntry
    {
        public bool Enable
        {
            get => _enable;
            set
            {
                _enable = value;
                OnPropertyChanged("Enable", TriggerUpdate.Now);
            }
        }

        public string Input
        {
            get => _input;
            set
            {
                if (value.Length > 10)
                {
                    _input = value.Substring(0, 10);
                }
                else
                {
                    _input = value;
                }
                OnPropertyChanged("Input", TriggerUpdate.Now);
            }
        }

        public string Output
        {
            get => _output;
            set
            {
                _output = value;
                OnPropertyChanged("Output", TriggerUpdate.Now);
            }
        }

        public ReplacePhrase()
        {
        }

        public ReplacePhrase(string input, string output)
        {
            Input = input;
            Output = output;
        }

        protected override IEnumerable<Trigger> GenerateTriggers()
        {
            if (Enable && !HasErrors)
            {
                return new List<InputTrigger>
                {
                    new InputTrigger(Input)
                    {
                        TriggerType = TriggerType.PhraseReplace,
                        Operation = new SendText
                        {
                            Text = Output,
                            BackspaceCount = Input.Length
                        }
                    }
                };
            }
            return null;
        }

        private bool _enable = true;

        private string _input;

        private string _output;
    }
}
