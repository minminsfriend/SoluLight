using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Geminapi
{
    public partial class FileMan : Form
    {
        const string 닫기= "닫기";
        const string 삭제= "삭제";
        const string 아래로= "아래로";
        const string 위로= "위로";
        const string 상위폴더= "상위폴더";
        const string 폴더진입= "폴더진입";
        const string 아이디선택= "아이디선택";
        const string 화일선택= "화일선택";
        const string 폴더선택= "폴더선택";
        const string 폴더생성= "폴더생성";
        const string 이름변경= "이름변경";
        const string 멀티변경= "멀티변경";
        const string 화일복제 = "화일복제";


        string dirApi = @"d:\Works\ApiGemini";
        string dirCurr = @"d:\Works\ApiGemini";
        List<string> filesInCurrDir = new List<string>();

        Dictionary<string, int> logIndexSel = new Dictionary<string, int>();
        Dictionary<string, int> logIndexStart = new Dictionary<string, int>();

        int indexSelected = -1;
        int indexFirstPrint = -1;
        readonly int numsOfVisibleTems = 20;
        readonly int HeightTem = 30;

        string nameKeyCurr;
        Geminapi main;

        public FileMan(Geminapi main)
        {
            this.main = main;

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
            string pathSel= filesInCurrDir.Count == 0 ? null :filesInCurrDir[indexSelected];

            string command = key switch
            {
                Keys.Escape => 닫기,
                Keys.Delete => 삭제,
                Keys.Down => 아래로,
                Keys.Up => 위로,
                Keys.Back => 상위폴더,
                Keys.Enter => Directory.Exists(pathSel) ? 폴더진입 : 화일선택,
                Keys.Space => Directory.Exists(pathSel) ? 폴더선택 : 아이디선택,
                Keys.F7 => 폴더생성,
                Keys.F2 => 이름변경,
                Keys.F3 => 멀티변경,
                Keys.C => 화일복제,
                _ => null,
            };

            if (command == null)
                return;

            switch (command)
            {
                case 닫기:
                    this.Hide();
                    return;
                case 폴더생성:
                    CreateNewFolder();
                    LoadDirectory(dirCurr);
                    return;
                case 상위폴더:
                    {
                        DirectoryInfo parent = Directory.GetParent(dirCurr);
                        if (parent != null)
                        {
                            LoadDirectory(parent.FullName);
                        }
                        else
                        {
                            MessageBox.Show("최상위 루트 디렉토리입니다.");
                        }

                        return;
                    }
            }

            if (pathSel == null) 
                return;

            switch (command)
            {
                case 아래로:
                    if (indexSelected < filesInCurrDir.Count - 1)
                    {
                        indexSelected++;
                        if (indexSelected >= indexFirstPrint + numsOfVisibleTems)
                            indexFirstPrint++;

                        this.Invalidate();
                    }
                    break;
                case 위로:
                    if (indexSelected > 0)
                    {
                        indexSelected--;
                        if (indexSelected < indexFirstPrint)
                            indexFirstPrint--;

                        this.Invalidate();
                    }
                    break;
                case 상위폴더:
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
                case 폴더진입:
                    if (Directory.Exists(pathSel))
                    {
                        // 폴더이면 진입
                        LoadDirectory(pathSel);
                    }
                    break;
                case 화일선택:
                    if (File.Exists(pathSel))
                    {
                        this.Hide();
                        main.화일관리자로부터(nameKeyCurr, pathSel, command);
                    }
                    break;
                case 아이디선택:
                    if (File.Exists(pathSel))
                    {
                        //this.Hide();
                        main.화일관리자로부터(nameKeyCurr, pathSel, command);

                        if (indexSelected < filesInCurrDir.Count - 1)
                        {
                            indexSelected++;
                            if (indexSelected >= indexFirstPrint + numsOfVisibleTems)
                                indexFirstPrint++;

                            this.Invalidate();
                        }
                    }
                    break;
                case 폴더선택:
                    if (Directory.Exists(pathSel))
                    {
                        this.Hide();
                        main.화일관리자로부터(nameKeyCurr, pathSel, command);
                    }
                    break;
                case 이름변경:
                    if (Directory.Exists(pathSel))
                        RenameDirectory(pathSel);
                    else
                        RenameFile(pathSel);
                    LoadDirectory(dirCurr);
                    break;
                case 멀티변경:
                    RenameMultiFiles(pathSel);
                    LoadDirectory(dirCurr);
                    break;
                case 화일복제:
                    CopyFile(pathSel);
                    LoadDirectory(dirCurr);
                    break;
                case 삭제:
                    DeleteTarget(pathSel);
                    LoadDirectory(dirCurr);
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
        }
        void RenameMultiFiles(string oldPath)
        {
            string oldNude = Path.GetFileNameWithoutExtension(oldPath);
            string dirPath = Path.GetDirectoryName(oldPath);

            Point loca = getInputLocation(indexSelected, indexFirstPrint);

            using (var dialog = new MyInput("멀티 화일들 변경", oldNude, loca))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 1. 개행 문자 제거 및 공백 정리
                    string text = dialog.InputText.Replace("\r", "").Replace("\n", "").Trim();

                    if (string.IsNullOrEmpty(text)) return;

                    var ss = Regex.Split(text, "/");
                    if (ss.Length != 2)
                    {
                        MessageBox.Show("변경 형식이 올바르지 않습니다.\n예: oldname/newname");
                        return;
                    }

                    var oldpart = ss[0].Trim();
                    var newpart = ss[1].Trim();

                    if (newpart == oldpart)
                    {
                        MessageBox.Show("변경할 이름이 동일합니다.");
                        return;
                    }

                    var filesToRename = Directory.GetFiles(dirPath)
                        .Where(f => Path.GetFileName(f).Contains(oldpart))
                        .ToList();

                    foreach (var file in filesToRename)
                    {
                        var nameCurr = Path.GetFileName(file);
                        var nameNew = nameCurr.Replace(oldpart, newpart);
                        var pathNew = Path.Combine(dirPath, nameNew);
                        // 대상 파일이 이미 존재하는지 체크
                        if (File.Exists(pathNew))
                        {
                            MessageBox.Show($"이미 동일한 이름의 파일이 있습니다: {nameNew}");
                            continue;
                        }
                        File.Move(file, pathNew);
                    }
                }
            }
        }
        void RenameFile(string oldPath)
        {
            string oldName = Path.GetFileName(oldPath);
            Point loca = getInputLocation(indexSelected, indexFirstPrint);

            using (var dialog = new MyInput("파일명 변경", oldName, loca))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string newName = dialog.InputText.Replace("\r", "").Replace("\n", "").Trim();

                    if (string.IsNullOrEmpty(newName) || oldName == newName) return;

                    try
                    {
                        string newPath = Path.Combine(Path.GetDirectoryName(oldPath), newName);

                        if (File.Exists(newPath))
                        {
                            MessageBox.Show("이미 동일한 이름의 파일이 있습니다.");
                            return;
                        }

                        File.Move(oldPath, newPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"변경 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        void RenameDirectory(string oldPath)
        {
            // 1. 폴더명 추출 (Path.GetFileName은 경로의 마지막 요소인 폴더명도 잘 가져옵니다)
            string oldName = Path.GetFileName(oldPath);
            Point loca = getInputLocation(indexSelected, indexFirstPrint);

            using (var dialog = new MyInput("폴더명 변경", oldName, loca))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string newName = dialog.InputText.Replace("\r", "").Replace("\n", "").Trim();

                    // 변경사항이 없거나 빈 문자열이면 리턴
                    if (string.IsNullOrEmpty(newName) || oldName == newName) return;

                    try
                    {
                        // 2. 부모 경로와 새 폴더명 결합
                        string parentPath = Path.GetDirectoryName(oldPath);
                        string newPath = Path.Combine(parentPath, newName);

                        // 3. 대상 폴더가 이미 존재하는지 확인
                        if (Directory.Exists(newPath))
                        {
                            MessageBox.Show("이미 동일한 이름의 폴더가 존재합니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // 4. 폴더 이동(이름 변경) 실행
                        Directory.Move(oldPath, newPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"폴더명 변경 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        internal void FeedFileIds(Dictionary<string, string> fileIds)
        {
            var dirIds = $@"{dirApi}\fileIds";

            var files = Directory.GetFiles(dirIds);

            foreach (var file in files)
            {
                File.Delete(file);
            }

            foreach (var kvp in fileIds)
            {
                var path = $@"{dirIds}\{kvp.Key}";
                File.WriteAllText(path, kvp.Value, Encoding.UTF8);
            }

            LoadDirectory(dirIds);
            this.Invalidate();
            this.Show();
        }
        internal void FeedCashIds(Dictionary<string, string> cashIds)
        {
            var dirIds = $@"{dirApi}\cashIds";

            var files = Directory.GetFiles(dirIds);

            foreach (var file in files)
            {
                File.Delete(file);
            }

            foreach (var kvp in cashIds)
            {
                var path = $@"{dirIds}\{kvp.Key}";
                File.WriteAllText(path, kvp.Value, Encoding.UTF8);
            }

            LoadDirectory(dirIds);
            this.Invalidate();
            this.Show();
        }
    }
}