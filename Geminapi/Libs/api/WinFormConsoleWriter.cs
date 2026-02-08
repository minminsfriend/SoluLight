using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Geminapi
{
    public class WinFormConsoleWriter : TextWriter
    {
        private RichTextBox _output;

        public WinFormConsoleWriter(RichTextBox output)
        {
            _output = output;
        }

        // Console.Write/WriteLine이 호출될 때 이 메서드가 실행됨
        public override void WriteLine(string value)
        {
            // UI 스레드 안전하게 출력
            if (!_output.IsDisposed)
            {
                _output.BeginInvoke(new Action(() =>
                {
                    _output.AppendText(value + Environment.NewLine);
                    _output.ScrollToCaret();
                }));
            }
        }
        public override void Write(char value)
        {
            // UI 스레드 안전을 위해 Invoke 사용
            _output.BeginInvoke(new Action(() => {
                _output.AppendText(value.ToString());
            }));
        }

        // 2. Write(string) - 문자열 출력 (성능 및 편의를 위해 구현)
        public override void Write(string value)
        {
            if (value == null) return;
            _output.BeginInvoke(new Action(() => {
                _output.AppendText(value);
            }));
        }

        public override Encoding Encoding => Encoding.UTF8;
    }
}
