using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KPlayer
{
    public partial class FileMan : Form
    {
        string dirCurr = @"d:\Works\vs\SoluLight\KPlayer\Data\kidis";
        List<string> filesInCurrDir = new List<string>();

        Dictionary<string, int> logIndexSel = new Dictionary<string, int>();
        Dictionary<string, int> logIndexStart = new Dictionary<string, int>();

        // 인덱스 관리
        int indexSelected = -1;
        int indexFirstPrint = -1;
        readonly int numsOfVisibleTems = 20;
        readonly int HeightTem = 30;

        string nameKeyCurr;

        KPlayer hot;

        public FileMan(KPlayer hot)
        {
            this.hot = hot;

            InitializeComponent();

            this.Text = "미니 관리자";
            //this.Size = new Size(500, 800);
            //this.Location = new Point(800, 50);
            this.DoubleBuffered = true;
            this.KeyPreview = true; // 폼에서 키 입력을 먼저 가로챔

            LoadDirectory(dirCurr);
            this.KeyDown += FileMan_KeyDown;
            this.KeyUp += FileMan_KeyUp;
            this.Paint += FileMan_Paint;
        }
        void FileMan_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);

            drawFilesList(g);

            Font font = new Font("맑은 고딕", 10);

            // 600y 경계선 및 하단 정보창
            g.DrawLine(Pens.Gray, 0, 600, 500, 600);
            g.FillRectangle(Brushes.GhostWhite, 0, 601, 500, 200);
            g.DrawString($"Total: {filesInCurrDir.Count} | Selected Index: {indexSelected}",
                font, Brushes.Black, 10, 615);
            g.DrawString($"Path: {dirCurr}", font, Brushes.DimGray, 10, 640);
        }
        void FileMan_KeyUp(object sender, KeyEventArgs e)
        {

        }
        void FileMan_KeyDown(object sender, KeyEventArgs e)
        {
            var key = e.KeyCode;
            string targetPath;
            switch (key)
            {
                case Keys.Escape:
                    this.Hide();
                    break;
                case Keys.Delete:
                    if (filesInCurrDir.Count > 0)
                    {
                        targetPath = filesInCurrDir[indexSelected];
                        DeleteTarget(targetPath);
                    }
                    break;
                case Keys.Down:
                    if (filesInCurrDir.Count > 0 && indexSelected < filesInCurrDir.Count - 1)
                    {
                        indexSelected++;
                        if (indexSelected >= indexFirstPrint + numsOfVisibleTems)
                            indexFirstPrint++;

                        this.Invalidate();
                    }
                    break;
                case Keys.Up:
                    if (filesInCurrDir.Count > 0 && indexSelected > 0)
                    {
                        indexSelected--;
                        if (indexSelected < indexFirstPrint)
                            indexFirstPrint--;

                        this.Invalidate();
                    }
                    break;
                case Keys.Back:
                    DirectoryInfo parent = Directory.GetParent(dirCurr);
                    if (parent != null)
                    {
                        LoadDirectory(parent.FullName);
                    }
                    else
                    {
                        MessageBox.Show("최상위 루트 디렉토리입니다.");
                    }
                    break;
                case Keys.Enter:
                    if (filesInCurrDir.Count == 0) return;

                    targetPath = filesInCurrDir[indexSelected];

                    if (Directory.Exists(targetPath))
                    {
                        // 폴더이면 진입
                        LoadDirectory(targetPath);
                    }
                    else if (File.Exists(targetPath))
                    {
                        this.Hide();
                        hot.fm_doSomething(nameKeyCurr, targetPath);
                    }
                    break;
                case Keys.F7:
                    CreateNewFolder();
                    break;
                case Keys.F2:
                    targetPath = filesInCurrDir[indexSelected];

                    RenameFile(targetPath);
                    break;
                case Keys.C:
                    targetPath = filesInCurrDir[indexSelected];

                    CopyFile(targetPath);
                    break;
            }
        }
        void LoadDirectory(string path)
        {
            try
            {
                if (Directory.Exists(dirCurr))
                {
                    logIndexSel[dirCurr] = indexSelected;
                    logIndexStart[dirCurr] = indexFirstPrint;
                }

                if (Directory.Exists(path))
                {
                    if (logIndexSel.ContainsKey(path))
                        indexSelected = logIndexSel[path];
                    if (logIndexStart.ContainsKey(path))
                        indexFirstPrint = logIndexStart[path];

                    dirCurr = path;
                    filesInCurrDir = Directory.GetFileSystemEntries(path).OrderBy(f => Path.GetExtension(f)).ToList();

                    if (filesInCurrDir.Count == 0)
                        indexSelected = -1;
                    else if (indexSelected > filesInCurrDir.Count - 1)
                        indexSelected = filesInCurrDir.Count - 1;
                    else
                        indexSelected = Math.Max(0, indexSelected);

                    if (filesInCurrDir.Count == 0)
                        indexFirstPrint = -1;
                    else if (indexFirstPrint > filesInCurrDir.Count - 1)
                        indexFirstPrint = filesInCurrDir.Count - 1;
                    else
                        indexFirstPrint = Math.Max(0, indexFirstPrint);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            this.Invalidate();
        }
        void CopyFile(string path)
        {
            if (Directory.Exists(path))
                return;

            var dir = Path.GetDirectoryName(path);
            var name = Path.GetFileNameWithoutExtension(path);
            var ext = Path.GetExtension(path);

            var pathNew = $@"{dir}\{name}_복사본{ext}";

            File.Copy(path, pathNew);
            LoadDirectory(dirCurr);
        }
        void RenameFile(string oldPath)
        {
            string oldName = Path.GetFileName(oldPath);

            Point loca = getInputLocation(indexSelected, indexFirstPrint);

            using (var dialog = new MyInput("파일명 변경", oldName, loca))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 1. 개행 문자 제거 및 공백 정리
                    string newName = dialog.InputText.Replace("\r", "").Replace("\n", "").Trim();

                    if (string.IsNullOrEmpty(newName) || oldName == newName) return;

                    try
                    {
                        string newPath = Path.Combine(Path.GetDirectoryName(oldPath), newName);

                        // 2. 대상 파일이 이미 존재하는지 체크
                        if (File.Exists(newPath))
                        {
                            MessageBox.Show("이미 동일한 이름의 파일이 있습니다.");
                            return;
                        }

                        File.Move(oldPath, newPath);
                        LoadDirectory(dirCurr);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"변경 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        void doSomethin_InMainForm(string command, string targetPath)
        {

        }
        void CreateNewFolder()
        {
            Point loca = getInputLocation(indexSelected, indexFirstPrint);

            // 리치박스 다이얼로그 호출 (제목: 새 폴더 생성, 기본값: 새 폴더)
            using (var dialog = new MyInput("새 폴더 생성", "새 폴더", loca))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string folderName = dialog.InputText.Trim();

                    if (string.IsNullOrEmpty(folderName)) return;

                    try
                    {
                        // 현재 경로와 입력된 폴더명을 결합
                        string newFolderPath = Path.Combine(dirCurr, folderName);

                        // 폴더가 이미 존재하는지 확인
                        if (!Directory.Exists(newFolderPath))
                        {
                            // 실제 폴더 생성 실행
                            Directory.CreateDirectory(newFolderPath);

                            // 리스트 갱신하여 화면에 새 폴더 표시
                            LoadDirectory(dirCurr);
                        }
                        else
                        {
                            MessageBox.Show("이미 같은 이름의 폴더가 존재합니다.", "알림");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"폴더 생성 실패: {ex.Message}", "오류");
                    }
                }
            }
        }
        void DeleteTarget(string targetPath)
        {
            // 1. 대상의 종류 판별
            bool isDirectory = Directory.Exists(targetPath);
            bool isFile = File.Exists(targetPath);

            if (!isDirectory && !isFile) return; // 대상이 존재하지 않으면 종료

            string typeName = isDirectory ? "폴더" : "파일";
            string name = Path.GetFileName(targetPath);

            // 2. 사용자 확인 다이얼로그
            string message = isDirectory
                ? $"정말로 이 폴더와 내부의 모든 항목을 삭제하시겠습니까?\n\n이름: {name}"
                : $"정말로 이 파일을 삭제하시겠습니까?\n\n이름: {name}";

            DialogResult result = MessageBox.Show(
                message,
                $"{typeName} 삭제 확인",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);

            if (result == DialogResult.OK)
            {
                try
                {
                    if (isDirectory)
                    {
                        // Directory.Delete의 두 번째 인자를 true로 설정하면 
                        // 비어 있지 않은 폴더도 내부 파일과 함께 삭제됩니다.
                        Directory.Delete(targetPath, true);
                    }
                    else
                    {
                        File.Delete(targetPath);
                    }

                    // 3. 삭제 완료 후 리스트 갱신 및 인덱스 조정
                    LoadDirectory(dirCurr);

                    // 삭제된 후 선택 바가 리스트 범위를 벗어나지 않도록 보정
                    if (indexSelected >= filesInCurrDir.Count)
                    {
                        indexSelected = Math.Max(0, filesInCurrDir.Count - 1);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("삭제 권한이 없습니다. 관리자 권한으로 실행하거나 파일 속성을 확인하세요.", "권한 오류");
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"파일이 사용 중이거나 오류가 발생했습니다: {ex.Message}", "삭제 실패");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류");
                }
            }
        }
        void drawFilesList(Graphics g)
        {
            if (filesInCurrDir.Count == 0)
                return;

            Font font = new Font("맑은 고딕", 10);

            // 0 ~ 600y 영역에 리스트 그리기
            for (int i = 0; i < numsOfVisibleTems; i++)
            {
                int entryIndex = indexFirstPrint + i;
                if (entryIndex >= filesInCurrDir.Count) break;

                string tem = filesInCurrDir[entryIndex];
                string name = Path.GetFileName(tem);
                bool isDirectory = Directory.Exists(tem);

                int yPos = i * HeightTem;
                Rectangle itemRect = new Rectangle(5, yPos + 2, 475, HeightTem - 4);

                if (entryIndex == indexSelected)
                {
                    using (Pen selectPen = new Pen(Color.Red, 2))
                    {
                        g.DrawRectangle(selectPen, itemRect);
                    }
                }

                Brush textBrush = isDirectory ? Brushes.Blue : Brushes.Black;
                string prefix = isDirectory ? "▶ " : "◇   ";
                g.DrawString($"{prefix}{name}", font, textBrush, 15, yPos + 7);

                g.DrawLine(Pens.AliceBlue, 0, yPos + HeightTem, 500, yPos + HeightTem);
            }
        }
        void SendCommand(string command)
        {
            Point loca = getInputLocation(indexSelected, indexFirstPrint);

            using (var dialog = new MyInput("command", "text", loca))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string result = dialog.InputText;
                    MessageBox.Show("확인되었습니다: " + result);
                }
                else
                {
                    MessageBox.Show("취소되었습니다.");
                }
            }
        }
        Point getInputLocation(int indexSelected, int indexFirstPrint)
        {
            Point loca = this.Location;

            for (int i = 0; i < numsOfVisibleTems; i++)
            {
                int entryIndex = indexFirstPrint + i;
                if (entryIndex >= filesInCurrDir.Count) 
                    break;

                int yPos = i * HeightTem;
                Rectangle itemRect = new Rectangle(5, yPos + 2, 475, HeightTem - 4);

                if (entryIndex == indexSelected)
                {
                    loca.X += itemRect.X + 40;
                    loca.Y += itemRect.Y + 35;

                    break;
                }
            }

            return loca;
        }
        internal void ShowX(string nameKeyCurr, string dirStart)
        {
            this.nameKeyCurr = nameKeyCurr;
            this.dirCurr = dirStart;
            LoadDirectory(this.dirCurr);

            this.Show();
            this.Focus();
        }
    }
}