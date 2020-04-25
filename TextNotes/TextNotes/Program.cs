using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace TextNotes
{
    class Program
    {
        static void Main(string[] args)
        {
            var neededNewFile = true;
            var finishedLoadingFile = false;
            var note = new MyNote();
            var finishedWithNote = false;
            // get timestamp
            var timestamp = DateTime.Now;

            // check if folder exists for this week,
            // if not, create it
            string sourceDir = @"..\..\..\Notes";
            string monthDir = sourceDir + $@"\{timestamp.Year}-{timestamp.Date:MM}";
            Directory.CreateDirectory(monthDir);

            // check if text file exists for this day,
            // if not, create it
            string filename = $@"\{timestamp.DayOfWeek}-{timestamp.Day}.json";
            string path = monthDir + filename;
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {

                }
                Console.WriteLine($"New note file created at {path}");
            }
            else
            {
                Console.WriteLine($"Found note file for adding to at {path}");
                neededNewFile = false;
            }

            while (!finishedWithNote)
            {
                if (!neededNewFile || finishedLoadingFile)
                {
                    string input = string.Empty;
                    using (StreamReader sr = new StreamReader(path))
                    {
                        input = sr.ReadToEnd();
                    }
                    try
                    {
                        note = JsonConvert.DeserializeObject<MyNote>(input);
                        finishedLoadingFile = true;
                        var i = 0;
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                var entry = new Entry();
                Console.WriteLine("Please enter any applicable tags.");
                Console.WriteLine("Enter all tags, separated by commas, then hit enter.");
                var tagString = Console.ReadLine();
                var splitTags = tagString.Split(',');
                var tags = new List<string>();
                foreach (var tag in splitTags)
                {
                    tags.Add(tag.Trim().ToLowerInvariant());
                }
                entry.Tags = tags;

                var doneWithComments = false;
                while (!doneWithComments)
                {
                    Console.WriteLine();
                    Console.WriteLine("Please enter a new comment.");
                    Console.WriteLine("Enter a comment, then hit enter. Type q to quit.");
                    var comment = Console.ReadLine();
                    if (comment == "q")
                    {
                        doneWithComments = true;
                        note.Entries.Add(entry);
                        //string output = JsonConvert.SerializeObject(note);
                        //Console.WriteLine(output);
                        WriteNote(note, path, false);
                        finishedLoadingFile = true;
                    }
                    else
                    {
                        entry.Comments.Add(comment);
                    }
                }
                Console.WriteLine();
                Console.WriteLine("===");
                Console.WriteLine();
            }
        }

        private static void WriteNote(MyNote note, string path, bool isAppend)
        {
            using (StreamWriter sw = new StreamWriter(path, isAppend))
            {
                
                sw.WriteLine(JsonConvert.SerializeObject(note));
            }
        }

        private class MyNote
        {
            public MyNote()
            {
                this.DateString = DateTime.Now.ToShortDateString();
                this.Entries = new List<Entry>();
            }
            public string DateString { get; set; }
            public List<string> Tags
            {
                get
                {
                    var entryTags = new List<string>();
                    foreach (var entry in Entries)
                    {
                        foreach (var tag in entry.Tags)
                        {
                            if (!entryTags.Contains(tag))
                            {
                                entryTags.Add(tag);
                            }
                        }
                    }
                    return entryTags;
                }
            }
            public List<Entry> Entries { get; set; }
        }

        private class Entry
        {
            public Entry()
            {
                this.TimestampString = DateTime.Now.ToShortTimeString();
                this.Comments = new List<string>();
            }
            public string TimestampString { get; set; }
            public List<string> Tags { get; set; }
            public List<string> Comments { get; set; }

        }
    }
}
