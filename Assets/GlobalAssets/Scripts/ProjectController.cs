using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectController : MonoBehaviour
{
    // Singleton pattern
    public static ProjectController Instance { get; private set; }

    // Project information
    public string projectName;
    public int numberOfClasses { get { return classes.Count; } }
    public List<string> classes = new List<string>();
    public Dictionary<string, string> classesToControlsMap = new Dictionary<string, string>();

    // Ensure only one instance of ProjectController exists
    private void Awake()
    {
        if (Instance == null)
        {
            // Set this instance as the singleton instance
            Instance = this;

            // Keep this object alive throughout the project
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy duplicate instances
            Destroy(gameObject);
        }
    }
}
