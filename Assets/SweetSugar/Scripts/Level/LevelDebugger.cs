using System;
using System.IO;
using UnityEngine;

//using UnityEditor;

namespace SweetSugar.Scripts.Level
{
    public class LevelDebugger {

        public static void SaveMap(int[] items, int maxCols, int maxRows) {
            var saveString = "";
            var filename = "levelstate.txt";


            //set map data
            for (var row = 0; row < maxRows; row++) {
                for (var col = 0; col < maxCols; col++) {
                    saveString += items[row * maxCols + col];
                    saveString += " ";
                }
            }
            //Write to file
            var activeDir = Application.dataPath + @"/SweetSugar/Resources/";
            var newPath = Path.Combine(activeDir, filename + ".txt");
            var sw = new StreamWriter(newPath);
            sw.Write(saveString);
            sw.Close();
            //AssetDatabase.Refresh();

        }

        public static int[] LoadMap(int maxCols, int maxRows) {
            var filename = "levelstate.txt";
            var items = new int[99];
            var row = 0;
            var mapText = Resources.Load(filename) as TextAsset;
            var filetext = mapText.text;
            var lines = filetext.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines) {
                var st = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                for (var i = 0; i < st.Length; i++) {
                    items[row * maxCols + i] = int.Parse(st[i]);
                }
                row++;
            }
            return items;

        }
    }
}
