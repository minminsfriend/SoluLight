using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using shine.libs.math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MidiFile = Melanchall.DryWetMidi.Core.MidiFile;

namespace KPlayer
{
	public enum SortMode
	{
		ByNameAscending,
		ByNameDescending,
		// 필요 시 크기나 날짜별 정렬 추가 가능
	}
	public class KPlayer00
	{
		int selectedIndex = 0; // 현재 선택된 항목의 인덱스 (0부터 시작)
		List<string> fileList = new List<string>(); // 실제 파일 목록 데이터
		bool isCtrlOnly;
		Panel panel1, panel2;
		bool isRenamingMode = false;
		string currentFolder = "C:\\MyMidiFiles";

		string currentDirectoryPath;
		List<string> fileList2 = new List<string>();
		int selectedIndex2 = 0;

		// 필터링 및 정렬 상태를 저장할 변수 추가
		string currentFilter = "*.*"; // 기본값은 모든 파일
		System.IO.SearchOption searchOption = System.IO.SearchOption.TopDirectoryOnly;
		SortMode currentSortMode = SortMode.ByNameAscending; // 기본 정렬 방식

		List<string> allMidis = new List<string> { "Chopin_Nocturne.txt", "Beethoven_Symphony.mid", "Jazz_Impro.txt", "Basic_Chord.mid" };
		List<string> displayList;
		void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey)
			{
				isCtrlOnly = true;
			}
			else
			{
				isCtrlOnly = false;

				// 중요: Ctrl 이외의 키가 눌리면 RichTextBox에 입력되도록 허용
				e.SuppressKeyPress = false;
			}
		
			if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey)
			{
				if (isCtrlOnly)
				{
					// 여기에 '기능 모드' (예: 저장, 불러오기 등) 실행 로직
					MessageBox.Show("기능 모드 활성화!");

					// RichTextBox로 이벤트가 전달되지 않게 막음
					e.SuppressKeyPress = true;
				}
				isCtrlOnly = false;
			}
		
			switch (e.KeyCode)
			{
				case Keys.Up:
					if (selectedIndex > 0)
					{
						selectedIndex--;
						Invalidate(); // 화면 갱신 요청 (OnPaint 다시 호출)
					}
					break;

				case Keys.Down:
					if (selectedIndex < fileList.Count - 1)
					{
						selectedIndex++;
						Invalidate();
					}
					break;

				case Keys.Home:
					selectedIndex = 0;
					Invalidate();
					break;

				case Keys.End:
					selectedIndex = fileList.Count - 1;
					Invalidate();
					break;

				case Keys.Enter:
					// 선택된 파일 실행 (구현 필요)
					ExecuteSelectedFile(fileList[selectedIndex]);
					break;

				case Keys.Escape:
					// 폼 닫기 또는 다른 동작
					this.Hide();
					break;
			}

			// RichTextBox로 이벤트 전달 방지 (필요 시)
			e.SuppressKeyPress = true;
		
			// RichTextBox로 이벤트가 전달되지 않게 항상 차단
			e.SuppressKeyPress = true;

			// 이름 변경 모드일 때는 엔터나 ESC만 처리하고 나머지는 텍스트 입력으로 넘김
			if (isRenamingMode)
			{
				if (e.KeyCode == Keys.Enter)
				{
					ApplyRename();
				}
				else if (e.KeyCode == Keys.Escape)
				{
					CancelRename();
				}
				// 다른 키는 RichTextBox가 받아서 텍스트 입력으로 사용하도록 둠 (Suppress=false 처리 필요)
				return;
			}

			switch (e.KeyCode)
			{
				// --- 탐색 및 기본 기능 (Up/Down/Enter/Back/Tab) ---
				// ... (이전 코드 유지) ...

				// --- 파일 관리 기능 키 ---
				case Keys.Delete: // Delete 키 (삭제)
				case Keys.F2:     // F2 키 (사용자 요청 기준 삭제)
					if (fileList.Count > 0)
					{
						DeleteSelectedFile(fileList[selectedIndex]);
					}
					break;

				case Keys.F5:     // F5 키 (복사)
					if (fileList.Count > 0)
					{
						CopySelectedFile(fileList[selectedIndex]);
					}
					break;

				case Keys.F4:     // F4 또는 다른 키 (이름 변경 시작 - 표준은 F2)
					StartRenameMode();
					break;
			}

			Invalidate();
		
			// ... (이름 변경 모드 체크 및 다른 키 로직 유지) ...

			switch (e.KeyCode)
			{
				// ... (Up, Down, Enter, Back, Tab, Delete, F2 로직 유지) ...

				case Keys.F5:     // F5 키 (복사 및 이름 변경 모드 진입)
					if (fileList.Count > 0)
					{
						string selectedPath = fileList[selectedIndex];
						CopyAndRenameFile(selectedPath);
					}
					break;
			}

			Invalidate();

			// SubForm_KeyDown 메서드 내 switch (e.KeyCode) 문에 추가
			switch (e.KeyCode)
			{
				// ... (기존 Up, Down, Enter, Back, Tab, Delete, F2, F5 로직 유지) ...

				case Keys.F7: // F7 키 (새 폴더 생성)
					CreateNewFolder(); // 새 폴더 생성 함수 호출
					break;

				case Keys.F8: // F8 키 (잘라내기/이동)
					if (fileList.Count > 0)
					{
						MoveSelectedFile(fileList[selectedIndex]); // 이동 함수 호출
					}
					break;

					// Shift + Delete 조합 (영구 삭제)
					// switch 문 밖에서 e.Modifiers를 사용하여 별도 처리하는 것이 더 정확합니다.
			}

			// SubForm_KeyDown 메서드 내 switch 문 다음에 추가
			if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Delete)
			{
				if (fileList.Count > 0)
				{
					PermanentDeleteSelectedFile(fileList[selectedIndex]); // 영구 삭제 함수 호출
				}
				e.SuppressKeyPress = true; // 이벤트 추가 차단
			}
		
			// RichTextBox로 이벤트가 전달되지 않게 항상 차단
			e.SuppressKeyPress = true;

			switch (e.KeyCode)
			{
				// --- 탐색 키 ---
				case Keys.Up:
					// ... [위로 이동 로직] ...
					break;

				case Keys.Down:
					// ... [아래로 이동 로직] ...
					break;

				// --- 토탈커맨더 기능 키 ---

				case Keys.Enter:
					// 엔터: 폴더 진입 또는 파일 실행
					if (fileList.Count > 0)
					{
						string selectedPath = fileList[selectedIndex];
						ExecuteSelectedFile2(selectedPath);
					}
					break;

				case Keys.Back:
					// 백스페이스: 상위 폴더로 이동
					GoToParentFolder(); // 상위 폴더 이동 함수 호출 (구현 필요)
					break;

				case Keys.Tab:
					// 탭: 좌우 패널 포커스 이동 (듀얼 패널 구현 시)
					SwitchPanelFocus(); // 포커스 전환 함수 호출 (구현 필요)
					break;

					// ... 다른 단축키 (F1~F12 기능키 등) ...
			}

			// 화면 갱신
			Invalidate();
	
			// RichTextBox로 이벤트가 전달되지 않게 항상 차단
			e.SuppressKeyPress = true;

			// 이름 변경 모드일 때는 엔터나 ESC만 처리하고 나머지는 텍스트 입력으로 넘김
			if (isRenamingMode)
			{
				if (e.KeyCode == Keys.Enter)
				{
					ApplyRename();
				}
				else if (e.KeyCode == Keys.Escape)
				{
					CancelRename();
				}
				return;
			}

			switch (e.KeyCode)
			{
				// --- 탐색 및 기본 기능 (Up/Down/Enter/Back/Tab) ---
				// ... (이전 코드 유지) ...

				// --- 파일 관리 기능 키 ---
				case Keys.Delete: // Delete 키 (삭제)
					if (fileList.Count > 0)
					{
						DeleteSelectedFile(fileList[selectedIndex]);
					}
					break;

				case Keys.F2:     // F2 키 (이름 변경 시작 - Windows 표준)
					StartRenameMode();
					break;

				case Keys.F5:     // F5 키 (복사)
					if (fileList.Count > 0)
					{
						CopySelectedFile(fileList[selectedIndex]);
					}
					break;
			}

			Invalidate();
		
			// RichTextBox로 이벤트가 전달되지 않게 항상 차단
			e.SuppressKeyPress = true;

			// 이름 변경 모드일 때는 엔터나 ESC만 처리하고 나머지는 텍스트 입력으로 넘김
			if (isRenamingMode)
			{
				if (e.KeyCode == Keys.Enter)
				{
					ApplyRename();
				}
				else if (e.KeyCode == Keys.Escape)
				{
					CancelRename();
				}
				return;
			}

			switch (e.KeyCode)
			{
				// ... (Up, Down, Back, Tab 키 로직 유지) ...

				case Keys.Enter:
					// 엔터: 폴더는 진입, 파일은 이름 변경 모드 진입
					if (fileList.Count > 0)
					{
						string selectedPath = fileList[selectedIndex];
						if (System.IO.Directory.Exists(selectedPath))
						{
							// 폴더인 경우: 해당 폴더로 진입
							LoadFileList(selectedPath);
							selectedIndex = 0;
						}
						else if (System.IO.File.Exists(selectedPath))
						{
							// 파일인 경우: F2와 동일하게 이름 변경 모드 시작
							StartRenameMode();
						}
					}
					break;

				case Keys.Delete:
				case Keys.F2: // F2는 이제 이름 변경 모드 시작 기능으로만 사용
					StartRenameMode();
					break;

				case Keys.F5:
					// ... (복사 로직 유지) ...
					break;
			}

			Invalidate();
		}
		void ExecuteSelectedFile(string v)
		{
			throw new NotImplementedException();
		}
		void Invalidate()
		{
			throw new NotImplementedException();
		}
		void Hide()
		{
			throw new NotImplementedException();
		}
		void Close()
		{
			throw new NotImplementedException();
		}
		void ExecuteSelectedFile2(string path)
		{
			if (System.IO.Directory.Exists(path))
			{
				// 폴더일 경우: 해당 폴더로 경로 변경 및 목록 갱신
				LoadFileList(path); // 현재 목록을 새 폴더 내용으로 변경
				selectedIndex = 0; // 선택 인덱스 초기화
			}
			else if (System.IO.File.Exists(path))
			{
				// 파일일 경우: 파일 실행 (시스템 기본 앱으로 열기)
				try
				{
					System.Diagnostics.Process.Start(path);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"파일 실행 실패: {ex.Message}");
				}
			}
		}
		void LoadFileList(string path)
		{
			throw new NotImplementedException();
		}
		void GoToParentFolder()
		{
			string currentDirectory = "";/* 현재 폴더 경로를 저장하는 변수 */;
			System.IO.DirectoryInfo parentDir = System.IO.Directory.GetParent(currentDirectory);

			if (parentDir != null)
			{
				LoadFileList(parentDir.FullName); // 상위 폴더 경로로 목록 갱신
				selectedIndex = 0; // 선택 인덱스 초기화
			}
		}
		void SwitchPanelFocus()
		{
			if (panel1.Focused)
			{
				panel2.Focus();
			}
			else
			{
				panel1.Focus();
			}
		//}
		//void StartRenameMode()
		//{
			if (fileList.Count == 0) return;

			isRenamingMode = true;

			// 1. 기존 선택 숨기기 (raw drawing 영역)
			// Invalidate()를 호출하여 OnPaint에서 선택 하이라이트를 잠시 그리지 않도록 로직 수정

			// 2. RichTextBox 보이기 및 포커스 주기 (미리 폼에 배치되어 있어야 함)
			// 예시: this.fileNameInput.Visible = true;
			// 예시: this.fileNameInput.Text = System.IO.Path.GetFileName(fileList[selectedIndex]);
			// 예시: this.fileNameInput.Focus();

			// 이 시점부터 RichTextBox가 키 입력을 받기 시작합니다.
		}
		void StartRenameMode()
		{
			if (fileList.Count == 0) return;

			isRenamingMode = true;
			string selectedPath = fileList[selectedIndex];

			// this.fileNameInput.Visible = true;
			// this.fileNameInput.Focus();

			// 이름 변경 입력창에 확장자 없는 이름만 미리 채워두기
			// this.fileNameInput.Text = System.IO.Path.GetFileNameWithoutExtension(selectedPath);

			// 텍스트 박스 내용 전체 선택 (커서 이동 편의)
			// this.fileNameInput.SelectAll();
		}
		void ApplyRename()
		{
			// 이름 변경 로직 처리 후 모드 해제
			isRenamingMode = false;
			// this.fileNameInput.Visible = false;
			// Focus 다시 파일 리스트 컨트롤로
			// Invalidate();
		
			// 입력창에서 새 이름 가져오기 (가정: fileNameInput RichTextBox 사용)
			string newName = "";// fileNameInput.Text; 
			string currentPath = "";// fileList[selectedIndex]; // 현재 파일 경로

			// TODO: 실제 이름 변경 로직 구현
			bool renameSucceeded = TryRenameFile(currentPath, newName);

			if (renameSucceeded)
			{
				// 성공 시, 메인 폼으로 전달할 정보 설정
				//this.CommandType = "Rename";
				//this.CommandArgument = newName; 
				//this.IsCommandSuccessful = true;
				//this.DialogResult = DialogResult.OK; // 폼 결과값 설정
			}
			else
			{
				//this.DialogResult = DialogResult.Cancel;
			}

			this.Close(); // 엔터 입력 후 즉시 서브폼 닫기
		
			// 입력창에서 새 이름 가져오기 (가정: rich RichTextBox 사용)
			string command = "Rename";
			string argument = ";//";// rich.Text;

			// TODO: 여기서 유효성 검사 (이름이 비어있지 않은지 등)
			if (string.IsNullOrEmpty(argument))
			{
				MessageBox.Show("새 파일 이름을 입력하세요.");
				return; // 유효성 검사 실패 시 함수 종료
			}

			try
			{
				// 메인 폼의 메서드를 직접 호출하여 명령 실행 및 후속 처리 위임
				// 이 시점에서 메인 폼의 파일 목록이 갱신될 수 있습니다.

				this.main_DoSomethingInMain(command, argument);

				// 명령 실행 후 서브폼 숨기기 (닫지 않음)
				this.Hide();

				// 이름 변경 모드 플래그 초기화 (다음 사용을 위해)
				isRenamingMode = false;
				// fileNameInput.Visible = false; // 입력창 숨기기
			}
			catch (Exception ex)
			{
				MessageBox.Show("메인 폼 명령 실행 중 오류: " + ex.Message);
			}
	
			//string command = "Rename"; // 실행할 명령 정의
			//string argument = "";// rich.Text; // RichTextBox에서 새 이름 가져오기

			// TODO: 여기서 유효성 검사 (이름이 비어있지 않은지 등)
			if (string.IsNullOrEmpty(argument))
			{
				MessageBox.Show("새 파일 이름을 입력하세요.");
				return;
			}

			try
			{
				// 메인 폼의 메서드를 직접 호출하여 명령 실행 및 후속 처리 위임
				this.main_DoSomethingInMain(command, argument);

				// 명령 실행 후 서브폼 닫기
				this.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show("메인 폼 명령 실행 중 오류: " + ex.Message);
			}
		}
		void CancelRename()
		{
			// 변경 취소 후 모드 해제
			isRenamingMode = false;
			// this.fileNameInput.Visible = false;
			// Focus 다시 파일 리스트 컨트롤로
			// Invalidate();
		
			// 변경 취소 후 모드 해제 및 폼 숨기기
			isRenamingMode = false;
			// this.fileNameInput.Visible = false;
			this.Hide();
		}
		void DeleteSelectedFile(string path)
		{
			if (MessageBox.Show($"'{System.IO.Path.GetFileName(path)}' 파일을 삭제하시겠습니까?", "삭제 확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				try
				{
					if (System.IO.File.Exists(path))
						System.IO.File.Delete(path);
					else if (System.IO.Directory.Exists(path))
						System.IO.Directory.Delete(path, true); // 폴더와 내용물 함께 삭제

					// 목록 갱신
					LoadFileList(/* 현재 폴더 경로 */);
				}
				catch (Exception ex)
				{
					MessageBox.Show("삭제 실패: " + ex.Message);
				}
			}
		}
		void LoadFileList()
		{
			throw new NotImplementedException();
		}
		void CopySelectedFile(string path)
		{
			// 복사 대상 폴더 선택 UI 띄우기 등의 추가 로직이 필요합니다.
			MessageBox.Show("F5: 복사 기능 구현 필요");
		}
		bool TryRenameFile(string currentPath, string newName)
		{
			throw new NotImplementedException();
		}
		void CreateNewFolder()
		{
			throw new NotImplementedException();
		}
		void CopyAndRenameFile(string selectedPath)
		{
			throw new NotImplementedException();
		}
		void CopyAndRenameFile2(string sourcePath)
		{
			string directory = System.IO.Path.GetDirectoryName(sourcePath);
			string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(sourcePath);
			string extension = System.IO.Path.GetExtension(sourcePath);
			string destinationPath = sourcePath;

			int copyIndex = 1;
			// 고유한 파일명 찾기 (예: file.txt -> file 복사본.txt -> file 복사본(2).txt)
			do
			{
				string suffix = (copyIndex == 1) ? " 복사본" : $" 복사본({copyIndex})";
				string newFileName = $"{fileNameWithoutExt}{suffix}{extension}";
				destinationPath = System.IO.Path.Combine(directory, newFileName);
				copyIndex++;
			} while (System.IO.File.Exists(destinationPath) || System.IO.Directory.Exists(destinationPath));

			try
			{
				// 실제 파일 복사
				if (System.IO.File.Exists(sourcePath))
				{
					System.IO.File.Copy(sourcePath, destinationPath);
				}
				// 폴더 복사는 별도 로직이 필요 (재귀 함수 등) - 여기서는 파일만 가정

				// 메인 폼에 목록 갱신 요청 (메인폼의 LoadFileList 함수 사용 가정)
				// this.main.LoadFileList(directory); 

				// 목록 갱신 후, 새 파일의 인덱스를 찾아 selectedIndex 설정 (구현 필요)
				// selectedIndex = fileList.IndexOf(destinationPath); 

				// 이름 변경 모드 자동 진입
				StartRenameMode();
			}
			catch (Exception ex)
			{
				MessageBox.Show("파일 복사 실패: " + ex.Message);
			}
		}
		void DeleteSelectedFile3(string path)
		{
			// 사용자에게 삭제 여부 확인
			if (MessageBox.Show($"'{System.IO.Path.GetFileName(path)}' 파일을 휴지통으로 이동하시겠습니까?", "삭제 확인",
								MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				try
				{
					// TODO: 실제 휴지통으로 보내는 로직 구현 (Windows API P/Invoke 필요)
					// 임시 방편으로 File.Delete 사용
					if (System.IO.File.Exists(path))
						System.IO.File.Delete(path);
					else if (System.IO.Directory.Exists(path))
						System.IO.Directory.Delete(path, false); // 빈 폴더만 삭제

					// 성공 시 메인폼에 목록 갱신 요청 (가정)
					// this.main.LoadFileList(currentDirectoryPath); 
				}
				catch (System.IO.IOException ex)
				{
					// 오류 발생 시 사용자에게 메시지 표시
					MessageBox.Show($"파일 삭제 실패 (사용 중): {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"파일 삭제 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
		void PermanentDeleteSelectedFile(string path)
		{
			// Shift+Delete (영구 삭제) 확인
			if (MessageBox.Show($"'{System.IO.Path.GetFileName(path)}' 파일을 영구적으로 삭제하시겠습니까? (복구 불가)", "영구 삭제 확인",
								MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				try
				{
					if (System.IO.File.Exists(path))
						System.IO.File.Delete(path);
					else if (System.IO.Directory.Exists(path))
						System.IO.Directory.Delete(path, true); // 내용물 포함 강제 삭제

					// 성공 시 메인폼에 목록 갱신 요청
					// this.main.LoadFileList(currentDirectoryPath);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"영구 삭제 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
		void MoveSelectedFile(string path)
		{
			// F8 이동 (잘라내기) 기능은 복사보다 복잡하며 대상 폴더 선택 UI가 필요합니다.
			// 여기서는 간단한 예시와 오류 처리 구조만 보여줍니다.
			MessageBox.Show("F8: 파일 이동 기능 구현 필요. 대상 폴더를 선택해주세요.");

			// 예시 try-catch 구조
			/*
			try
			{
				string destination = "D:\\Temp\\"; // 실제 사용자 입력 받아야 함
				System.IO.File.Move(path, System.IO.Path.Combine(destination, System.IO.Path.GetFileName(path)));
				// this.main.LoadFileList(currentDirectoryPath); // 목록 갱신
			}
			catch (Exception ex)
			{
				MessageBox.Show($"파일 이동 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			*/
		}
		void main_DoSomethingInMain(string command, string argument)
		{
			throw new NotImplementedException();
		}
	}
}