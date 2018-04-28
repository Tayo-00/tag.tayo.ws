using System;
using System.Collections.Generic;
using System.IO;

namespace TAGGER.Utils
{
    internal class SplitMap
    {
        public static void Split(int players, int mapId)
        {
            try
            {
                var combo = 0;
                var shared = true;
                var ignore = true;
                var path = "Temp/";
                var list = new List<StreamWriter>();
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var map = File.ReadAllLines(path + mapId);

                for (var i = 0; i < players; i++)
                {
                    var create = File.Create($"{path}{mapId}_{i}");
                    create.Close();
                    try
                    {
                        var file = new StreamWriter($"{path}{mapId}_{i}");
                        list.Add(file);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                var player = list.ToArray();

                foreach (var t in map)
                {
                    if (!shared)
                    {
                        var split = t.Split(',');
                        var checkCombo = split[3];
                        if (checkCombo == "4" || checkCombo == "5" || checkCombo == "6" || checkCombo == "70")
                        {
                            if (!ignore)
                            {
                                if (combo == players - 1)
                                {
                                    combo = 0;
                                }
                                else
                                {
                                    combo++;
                                }
                            }
                            player[combo].WriteLine(t);
                            ignore = false;
                        }
                        else
                        {
                            if (ignore)
                                ignore = false;
                            if (checkCombo == "8" || checkCombo == "12")
                            {
                                for (var j = 0; j < players; j++)
                                {
                                    player[j].WriteLine(t);
                                }
                            }
                            else
                            {
                                player[combo].WriteLine(t);
                            }
                        }
                    }
                    if (t.Contains("[HitObjects]"))
                    {
                        for (var j = 0; j < players; j++)
                        {
                            player[j].WriteLine(t);
                        }
                        shared = false;
                    }
                    if (shared)
                    {
                        for (var j = 0; j < players; j++)
                        {
                            player[j].WriteLine(t);
                        }
                    }
                }

                foreach (var t in player)
                {
                    t.Close();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
