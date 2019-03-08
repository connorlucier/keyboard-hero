using AudioHelm;
using System;
using TMPro;
using UnityEngine;

public class MidiStatsController : MonoBehaviour {

    public AudioHelmClock clock;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI accuracyText;

    [Range(4, 10)]
    public int maxMultiplier = 8;

    [Range(0, 1000)]
    public int scoreDensity = 1000;

    private long notesHit;
    private long totalNotes;
    private long score;
    private long streak;

    private double accuracy;

    private int multiplier;
    
    void Start()
    {
        ResetStats();
        UpdateStatsText();
    }

    private void ResetStats()
    {
        score = 0;
        streak = 0;
        notesHit = 0;
        totalNotes = 0;
        multiplier = 1;
        accuracy = 100.0;
    }

    public void NoteMissUpdate()
    {
        totalNotes++;
        streak = 0;
        multiplier = 1;
        accuracy = Math.Round(notesHit * 100.0 / totalNotes, 2);

        UpdateStatsText();
    }    

    public void NoteHitUpdate()
    {
        score += (int) (multiplier * scoreDensity * Time.deltaTime / (60 / clock.bpm));
        notesHit++;
        totalNotes++;
        streak++;
        multiplier = (int) Math.Min(1 + streak / 10, maxMultiplier);
        accuracy = Math.Round(notesHit * 100.0 / totalNotes, 2);

        UpdateStatsText();
    }

    public void NoteContinueUpdate()
    {
        score += (int) (multiplier * scoreDensity * Time.deltaTime / (60 / clock.bpm));
        UpdateStatsText();
    }

    public long Score() { return score; }
    public double Accuracy() { return accuracy; }

    private void UpdateStatsText()
    {
        scoreText.text = score.ToString();
        streakText.text = streak.ToString();
        accuracyText.text = accuracy.ToString() + "%";
    }
}
