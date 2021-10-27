using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Basic
{
   
    class Basic
    {
        static void Main(string[] args)
        {

            //PrintMe("testFile");
            //MyLs("./");
            //FillMe("testFile","test")
            //CopyMe("tesstFile","testFmmile");
            //enCryptoVernam("decrypted","AZDGKHHLMBHFPOTHATQYHCFHBOKBAAD","encrypted");
            //deCryptoVernam("encrypted","AZDGKHHLMBHFPOTHATQYHCFHBOKBAAD","decrypted");
        }

        public static void PrintMe(string path)
        {
            // TODO

            path = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")), path);
            if (!File.Exists(path))
            {
                Console.WriteLine($"Error : \"{path}\" does not exist");
                return;
            }

            StreamReader sr = File.OpenText(path);
            string s;
            int line = 1;
            while ((s = sr.ReadLine()) != null)
            {
                Console.WriteLine($"Line {line++}: {s}");
            }

            sr.Close();
        }

        public static void MyLs(string path)
        {
            string npath = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")), path);
            if (!Directory.Exists(npath))
            {
                Console.WriteLine($"Error: \"{path}\" is not a directory");
                return;
            }
            List<string> dirs = new List<string>(Directory.GetFileSystemEntries(npath));

            foreach (var dir in dirs)
            {
                Console.Write($"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 3)} ");
            }
        }

        public static void FillMe(string path, string content)
        {
            string Npath = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")), path);
            if (Directory.Exists(Npath))
            {
                Console.WriteLine("Directory with the same name already exists");
                return;

            }
            File.AppendAllText(Npath, string.Format("\n{0}", content));
            Console.WriteLine($"Printed \"{content}\" successfully in \"{path}\"");

        }

        public static void CopyMe(string path, string dest)
        {
            string npath = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")), path);
            string ndest = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")), dest);

            if (!File.Exists(npath))
            {
                Console.WriteLine($"Error: \"{path}\" does not exist, abort mission");
                return;
            }
            if (File.Exists(ndest))
            {
                Console.WriteLine($"Warning: The file \"{dest}\" already exists");
                Console.WriteLine(" Do you want to overwrite it ?");
                string res = Console.ReadLine();
                switch (res.ToLower())
                {
                    case "yes":
                    {
                        File.Copy(npath,ndest,true);
                        Console.WriteLine($"Copy from \"{path}\" to \"{dest}\" was successful");

                        break;
                    }
                    case "no":
                    {
                        Console.WriteLine("Mission aborted successfully");
                        return;
                    }
                    default:
                    {
                        Console.WriteLine("Error: bad input, you had one job...");
                        return;
                        
                    }
                    
                }
                return;
            }
            File.Copy(npath,ndest,true);
            Console.WriteLine($"Copy from \"{path}\" to \"{dest}\" was successful");
            
        }

        public static void enCryptoVernam(string decrypted, string key, string encrypted)
        {
            string path = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")), decrypted);
            string encryptedpath = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")), encrypted);
            if (!File.Exists(path))
            {
                Console.WriteLine($"Error : \"{decrypted}\" does not exist");
                return;
            }
            StreamReader sr = File.OpenText(path);
            string sourceText=sr.ReadLine();
            sr.Close();
            string encrypt = Encrypt_and_Decrypt(sourceText, key,true);
            StreamWriter sw = new StreamWriter(File.Open(encryptedpath, FileMode.Create));
            sw.WriteLine(encrypt);
            sw.Close();


        }

        public static void deCryptoVernam(string encrypted, string key, string decrypted)
        {
            string path = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")), encrypted);
            string decryptedpath = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")), decrypted);
            if (!File.Exists(path))
            {
                Console.WriteLine($"Error : \"{decrypted}\" does not exist");
                return;
            }
            StreamReader sr = File.OpenText(path);
            string sourceText=sr.ReadLine();
            sr.Close();
            string encrypt = Encrypt_and_Decrypt(sourceText, key,false);
            StreamWriter sw = new StreamWriter(File.Open(decryptedpath, FileMode.Create));
            sw.WriteLine(encrypt);
            sw.Close();
            
        }

        private static int Modulo(int k, int n)
        {
            return ((k %= n) < 0) ? k + n : k;
        }

        private static string Encrypt_and_Decrypt(string sourcetext, string key,bool mode)
        {
            string[] sourceWord = sourcetext.ToUpper().Split();

            char[] language = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            StringBuilder code = new StringBuilder();
            int[] key_id = new int[key.Length];
            
            for (int i = 0; i < key.Length; i++)
            {
                for (int j = 0; j < language.Length; j++)
                {
                    if (key[i] == language[j])
                    {
                        key_id[i] = j;
                        break;
                    }
                }
            }

            int indexInSourceText = 0;
            for (int id = 0; id < sourceWord.Length; id++)
            {
                string word = sourceWord[id];
            
                if (id != 0)
                {
                    code.Append(" ");
                }

                for (int i = 0; i < word.Length; i++)
                {
                
                    for (int j = 0; j < language.Length; j++)
                    {
               
                        if (word[i] == language[j])
                        {
                            if (mode)
                            {
                                code.Append(language[(j+key_id[indexInSourceText])%26]);
                     
                            }
                            else
                            {
                                
                              
                                code.Append(language[Modulo(j-key_id[indexInSourceText],26)]);
   
                            }
                            break;
                        }
                
                        else if (j == language.Length - 1)
                        {
                            code.Append(word[i]);
                        }
                    }

                    indexInSourceText++;
                }

            }
            
           

            return code.ToString();
        }
    }
}