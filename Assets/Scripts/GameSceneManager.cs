using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    private static readonly List<Note.Param> MUSIC_MASTER = new List<Note.Param>() {
        new Note.Param(Line.Line1, 1f),
        new Note.Param(Line.Line2, 2f),
        new Note.Param(Line.Line3, 3f),
        new Note.Param(Line.Line4, 4f),
        new Note.Param(Line.Line1, 5f),
        new Note.Param(Line.Line2, 6f),
        new Note.Param(Line.Line3, 7f),
        new Note.Param(Line.Line4, 8f),
    };

    private static readonly Dictionary<Line, float> LINE_POS = new Dictionary<Line, float>() {
        { Line.Line1, -6f },
        { Line.Line2, -2f },
        { Line.Line3, 2f },
        { Line.Line4, 6f },
    };

    private static readonly Dictionary<Judge, float> JUDGE_THRESHOLD = new Dictionary<Judge, float>() {
        { Judge.Perfect, 0.1f },
        { Judge.Great, 0.3f },
        { Judge.Good, 0.5f },
        { Judge.Miss, 1f },
    };

    private static readonly Dictionary<Judge, int> JUDGE_SCORE = new Dictionary<Judge, int>() {
        { Judge.Perfect, 1000 },
        { Judge.Great, 500 },
        { Judge.Good, 100 },
        { Judge.Miss, 0 },
    };

    private static readonly float BASE_SPEED = 10f;

    [SerializeField] private GameObject notePrefab;
    [SerializeField] private GameObject judgeLineObj;
    [SerializeField] private AudioSource bgm;
    [SerializeField] private AudioSource se;
    [SerializeField] private Text scoreText;
    [SerializeField] private JudgeText judgeText;
    [SerializeField] private ComboText comboText;

    private Dictionary<Line, List<Note>> notesDic;
    private Dictionary<Line, int> currentNoteIndexDic;

    private int score;
    private int combo;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Return)) {
        //    SceneManager.LoadScene("ResultScene");
        //}

        // ノート移動処理
        foreach (var notes in notesDic.Values) {
            foreach (var note in notes) {
                var pos = note.transform.position;
                pos.y = judgeLineObj.transform.position.y + (note.param.time - bgm.time) * BASE_SPEED;
                note.transform.position = pos;
            }
        }

        // 見逃しミス判定
        // FIXME: MUST リファクタリング！！！
        for (int i = 0; i < currentNoteIndexDic.Count; ++i) {
            var line = (Line)i;
            var index = currentNoteIndexDic[line];
            if (index < 0)
                continue;

            var note = notesDic[line][index];
            var diff = note.param.time - bgm.time;
            if (diff < -JUDGE_THRESHOLD[Judge.Miss]) {
                // ミス
                combo = 0;

                judgeText.Draw(Judge.Miss);
                comboText.Draw(combo);

                note.gameObject.SetActive(false);

                if (index + 1 < notesDic[line].Count) {
                    ++currentNoteIndexDic[line];
                } else {
                    currentNoteIndexDic[line] = -1;
                }
            }
        }

        // キー入力
        if (Input.GetKeyDown(KeyCode.A)) {
            JudgeNote(Line.Line1);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            JudgeNote(Line.Line2);
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            JudgeNote(Line.Line3);
        }
        if (Input.GetKeyDown(KeyCode.F)) {
            JudgeNote(Line.Line4);
        }
    }

    void Initialize() {
        notesDic = new Dictionary<Line, List<Note>>();
        currentNoteIndexDic = new Dictionary<Line, int>();
        foreach (Line line in System.Enum.GetValues(typeof(Line))) {
            notesDic.Add(line, new List<Note>());
            currentNoteIndexDic.Add(line, -1);
        }
        score = 0;
        combo = 0;

        // ノート生成
        foreach (var master in MUSIC_MASTER) {
            var obj = Instantiate(notePrefab, transform);
            var note = obj.GetComponent<Note>();
            note.Initialize(master);

            var pos = obj.transform.position;
            pos.x = LINE_POS[note.param.line];
            pos.y = judgeLineObj.transform.position.y + (BASE_SPEED * note.param.time);
            obj.transform.position = pos;

            notesDic[note.param.line].Add(note);
            if (currentNoteIndexDic[note.param.line] < 0) {
                currentNoteIndexDic[note.param.line] = 0;
            }
        }

        bgm.Play();
    }

    void JudgeNote(Line line) {
        se.Play();

        int index = currentNoteIndexDic[line];
        if (index < 0)
            return;

        var note = notesDic[line][index];
        var diff = Mathf.Abs(bgm.time - note.param.time);
        if (diff > JUDGE_THRESHOLD[Judge.Miss])
            return;

        var judge = Judge.Miss;
        if (diff < JUDGE_THRESHOLD[Judge.Perfect]) {
            judge = Judge.Perfect;
        } else if (diff < JUDGE_THRESHOLD[Judge.Great]) {
            judge = Judge.Great;
        } else if (diff < JUDGE_THRESHOLD[Judge.Good]) {
            judge = Judge.Good;
        }

        if (judge != Judge.Miss) {
            score += JUDGE_SCORE[judge];
            scoreText.text = "Score: " + score.ToString("D7");
            ++combo;
        } else {
            combo = 0;
        }

        judgeText.Draw(judge);
        comboText.Draw(combo);

        note.gameObject.SetActive(false);

        if (index + 1 < notesDic[line].Count) {
            ++currentNoteIndexDic[line];
        } else {
            currentNoteIndexDic[line] = -1;
        }
    }
}
