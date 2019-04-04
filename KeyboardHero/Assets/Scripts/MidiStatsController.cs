using AudioHelm;
using System;
using TMPro;
using UnityEngine;

public class MidiStatsController : MonoBehaviour {

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI accuracyText;

    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highStreakText;
    public TextMeshProUGUI totalAccuracyText;

    public AudioHelmClock clock;

    [Range(4, 10)]
    public int maxMultiplier = 8;

    [Range(0, 1000)]
    public int scoreDensity = 1000;

    private long notesHit;
    private long totalNotes;
    private long score;
    private long currentStreak;
    private long highStreak;

    private double accuracy;
    private float bpm;

    private int multiplier;
    
    void Start()
    {
        InitializeStats();
        bpm = clock.bpm;
    }

    public void InitializeStats()
    {
        score = 0;
        currentStreak = 0;
        highStreak = 0;
        notesHit = 0;
        totalNotes = 0;
        multiplier = 1;
        accuracy = 100.0;

        UpdateStatsText();
    }

    public void NoteHitUpdate()
    {
        score += (int) (multiplier * scoreDensity * Time.deltaTime / (60 / bpm));
        notesHit++;
        totalNotes++;
        currentStreak++;
        multiplier = (int) Math.Min(1 + currentStreak / 10, maxMultiplier);
        accuracy = Math.Round(notesHit * 100.0 / totalNotes, 2);

        UpdateStatsText();
    }

    public void NoteContinueUpdate()
    {
        score += (int) (multiplier * scoreDensity * Time.deltaTime / (60 / bpm));
        UpdateStatsText();
    }

    public void NoteMissUpdate()
    {
        if (currentStreak > highStreak)
            highStreak = currentStreak;

        totalNotes++;
        currentStreak = 0;
        multiplier = 1;
        accuracy = Math.Round(notesHit * 100.0 / totalNotes, 2);

        UpdateStatsText();
    }

    public void SetFinalStats()
    {
        if (currentStreak > highStreak)
            highStreak = currentStreak;

        finalScoreText.text = score.ToString();
        highStreakText.text = highStreak.ToString();
        totalAccuracyText.text = accuracy.ToString() + "%";
    }

    public long Score() { return score; }
    public double Accuracy() { return accuracy; }

    private void UpdateStatsText()
    {
        scoreText.text = score.ToString();
        streakText.text = currentStreak.ToString();
        accuracyText.text = accuracy.ToString() + "%";
    }
}
