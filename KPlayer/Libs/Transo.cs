using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPlayer
{
    public class KNoteEvasionEngine
    {
        private Random _rand = new Random();
        Dictionary<string, int[]> chords = new Dictionary<string, int[]>();

        public KNoteEvasionEngine()
        {
            FillChords();
        }
        void FillChords()
        {

            //chords["C major"] = new[] { 10, 30, 45 };
            //chords["C minor"] = new[] { 10, 25, 45 };
            //chords["D minor"] = new[] { 20, 35, 55 };
            //chords["E major"] = new[] { 30, 50, 65 };
            //chords["E minor"] = new[] { 30, 45, 65 };
            //chords["F major"] = new[] { 35, 55, 70 };
            //chords["G major"] = new[] { 45, 65, 80 };
            //chords["A minor"] = new[] { 55, 70, 90 };`

            chords["C major"] = new[] { 10, 20, 35 };
            chords["C minor"] = new[] { 10, 15, 35 };
            chords["D minor"] = new[] { 20, 15, 35 };
            chords["E major"] = new[] { 30, 20, 35 };
            chords["E minor"] = new[] { 30, 15, 35 };
            chords["F major"] = new[] { 35, 20, 35 };
            chords["G major"] = new[] { 45, 20, 35 };
            chords["A minor"] = new[] { 55, 15, 35 };
        }
        public KNote Transform(KNote knote)
        {
            long originalTime = knote.Time;
            int pitch = knote.Pitch;
            long duration = knote.Duration;
            int velocity = knote.Vel;
            bool isMel = knote.isMel;

            // 2. 피치 변조 (옥타브 하향 및 유효성 검증)
            // [규칙] x05와 같은 비유효 피치 생성 금지, 10~70 범위 준수
            int newPitch = pitch - 100;

            // 저작권 회피를 위한 미세 변조 (20% 확률로 5단위 비틀기)
            if (_rand.NextDouble() < 0.2)
            {
                int offset = _rand.Next(0, 2) == 0 ? 5 : -5;
                int tempPitch = newPitch + offset;

                // 유효 피치 검증 (끝자리가 10단위거나 5단위여야 함, 단 x05는 제외)
                int noteVal = tempPitch % 100;
                if (noteVal >= 10 && noteVal <= 70)
                {
                    newPitch = tempPitch;
                }
            }

            // 3. 리듬 변조 (매크로 시간 가산)
            // [규칙] 100ms 이상의 변위를 주어 지문 파괴
            int swing = (originalTime % 480 != 0) ? 120 : -40;
            long newTime = originalTime + swing + _rand.Next(-15, 15);

            // 4. 벨로시티 연산 (100 상한 엄격 준수)
            int newVelocity = Math.Min(100, velocity + _rand.Next(-10, 10));
            newVelocity = Math.Max(40, newVelocity); // 최소 가청 범위 유지

            // 5. 듀레이션 스케일링 (날카로운 질감 유도)
            int newDuration = (int)(duration * 0.9);

            // 6. 포스트 타임 설명 가산 및 출력 생성
            string comment = GetEvasionComment(newPitch, pitch - 100);

            int newChannel = 0;

            KNote kNote = new KNote(
                newTime,           // time
                newPitch,          // pitch
                newDuration,       // duration
                newVelocity,       // vel
                newChannel,       // channel
                isMel
            );

            return kNote;
        }

        private string GetEvasionComment(int currentPitch, int baseLoweredPitch)
        {
            if (currentPitch != baseLoweredPitch) return "선율 지문 변조 (Pitch Offset)";
            return "리듬 및 벨로시티 밸런싱 완료";
        }

        public List<string> GenerateArpeggio(string chordName, int startTime, int totalDuration, int patternSpeed = 200)
        {
            // 작가님의 chords 딕셔너리에서 화음 구성 음을 가져옴
            if (!chords.ContainsKey(chordName)) return new List<string>();
            int[] pitchOffsets = chords[chordName];

            List<string> arpeggioNotes = new List<string>();
            int currentTime = startTime;
            int patternIndex = 0;

            while (currentTime < startTime + totalDuration)
            {
                // 화음의 구성 음(Root, 15/20, 35)을 순차적으로 순환
                int currentPitch = pitchOffsets[patternIndex % pitchOffsets.Length];

                // KNote 포맷에 맞춰 생성 (반주이므로 isMel = 0)
                arpeggioNotes.Add($"{currentTime} : 音 {currentPitch} : 길이 {patternSpeed - 20} : Vel 70 : 0 : 자동 생성 반주");

                currentTime += patternSpeed;
                patternIndex++;
            }
            return arpeggioNotes;
        }
        // 특정 구간의 속도를 변환하고 이후의 모든 노트를 가산 처리함
        public void ApplySectionScale(List<KNote> notes, int startTime, int endTime, float scale)
        {
            long accumulatedOffset = 0;

            foreach (var note in notes)
            {
                if (note.Time >= startTime && note.Time <= endTime)
                {
                    // 구간 내 음들은 scale에 따라 길이와 위치가 변함
                    long originalDuration = note.Duration;
                    note.Duration = (long)(note.Duration * scale);

                    // 늘어난(혹은 줄어든) 시간만큼 오프셋 누적 (가산 연산)
                    accumulatedOffset += (note.Duration - originalDuration);
                }
                else if (note.Time > endTime)
                {
                    // 구간 이후의 음들은 누적된 오프셋만큼 뒤로 밀림 (Post-time 가산)
                    note.Time += accumulatedOffset;
                }
            }
        }

        public int QuantizePitch(int calculatedPitch)
        {
            // 옥타브 단위와 음 단위 분리
            int octave = (calculatedPitch / 100) * 100;
            int noteVal = calculatedPitch % 100;

            // 1. 최소/최대값 제한 (10~70)
            if (noteVal < 10) noteVal = 10;
            if (noteVal > 70) noteVal = 70;

            // 2. 5단위 정렬 (작가님의 5단위 피치 시스템 준수)
            noteVal = (int)(Math.Round(noteVal / 5.0) * 5);

            // 3. 최종 검증: 만약 75가 되면 다음 옥타브의 10으로 넘김
            if (noteVal > 70) { octave += 100; noteVal = 10; }

            return octave + noteVal;
        }
    }


}
