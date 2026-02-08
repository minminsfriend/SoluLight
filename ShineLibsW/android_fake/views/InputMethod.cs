using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.Content;
using Android.Graphics;

namespace Android.Views.InputMethods
{
    public interface IInputConnection
    {
        bool BeginBatchEdit();
        void CloseConnection();
        bool CommitText(string text, int newCursorPosition);

        bool DeleteSurroundingText(int beforeLength, int afterLength);
        bool DeleteSurroundingTextInCodePoints(int beforeLength, int afterLength);
        bool EndBatchEdit();

        bool FinishComposingText();

        bool PerformContextMenuAction(int id);
        bool PerformPrivateCommand(string action, Bundle data);

        bool ReportFullscreenMode(bool enabled);
        bool RequestCursorUpdates(int cursorUpdateMode);
        bool SendKeyEvent(KeyEvent e);

        bool SetComposingRegion(int start, int end);
        bool SetComposingText(string text, int newCursorPosition);
        bool SetSelection(int start, int end);
    }
}