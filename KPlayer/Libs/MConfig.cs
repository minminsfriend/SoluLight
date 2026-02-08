/*

# 미디 화일
곡 : Kalinka 세명의전차병6 Korobeiniki 슬라브여인2 슬라브여인M
맬 : 내멜로디4add2 내멜로디addout1 

# 단축키
%Q : 미디변경
%S : 저장노트
%M : 저장멜로디
%K : 갱신네임즈
%L : 로드미디
%P : 구간연주
%Z : 테스트
%U : 화일Add
            
X : 선택토글
C : 선택클리어
[ : 선택시작
] : 선택끝
M : 멜로디모드
P : 멜로디수정

*/

using System;
using System.Threading;
using System.Windows.Forms;

namespace KPlayer
{
    public class MConfig
    {
        string dirMidi = @"D:\Works\vs\SoluLight\KPlayer\Data\midis";
        string dirKidi = @"D:\Works\vs\SoluLight\KPlayer\Data\kidis";
        string filemidi = "미래가있다Mel.mid";
        string filetxtout = "미래가있다Mel.txt";
        string file4add = "내멜로디 4 add 2.txt";
        string fileaddout = "내멜로디 addout 1.txt";

        //string fileplay = "Kalinka.txt";
        //string fileplay = "세명의전차병6.txt";
        //string fileplay = "Korobeiniki.txt";
        string fileplay = "Korobeiniki멜.txt";
        //string fileplay = "슬라브여인2.txt";
        //string fileplay = "슬라브여인M.txt";
        //string fileplay = "내멜로디addout1.txt";
        //string fileplay = "미래가있다Mel.txt";

        string filesave = "저장.txt";
        string filesaveM = "멜로디.txt";

        //string midiScale = "6.0";
        string midiScale = "1.0";
        string filenames = @"D:\Works\vs\SoluLight\KPlayer\Libs\MConfig.cs";

        public MConfig()
        {

        }
    
    }
}
