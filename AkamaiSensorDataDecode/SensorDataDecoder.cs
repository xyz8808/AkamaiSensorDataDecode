namespace AkamaiSensorDataDecode
{
    public class SensorDataDecoder
    {
        string data;
        int step1_seed;
        int step2_seed;
        public SensorDataDecoder(string data)
        {
            this.data = data;
            var tmp = data.Split(';');
            step1_seed = Convert.ToInt32(tmp[2]);
            step2_seed = Convert.ToInt32(tmp[1]);
            this.data = string.Join(';', tmp.Skip(4));
        }

        public string[] Decode()
        {
            var arr = data.ToCharArray();
            var dict2 = MakeStep2(step2_seed);
            for (int i = 0; i < arr.Length; i++)
            {
                var c = arr[i];
                foreach (var item in dict2)
                {
                    if (item.Value[i] == c)
                    //var index = item.Value.FindIndex(o => o == arr[i]);
                    //if (index >= 0)
                    {
                        arr[i] = item.Key;
                    }
                }

            }
            var a = string.Join("", arr);
            var data_arr = a.Split(',');
            var dict1 = MakeStep1(data_arr,step1_seed);
            var r = dict1.ToList();
            r.Reverse();
            foreach (var item in r)
            {
                var Oqf = data_arr[item.Item1];
                data_arr[item.Item1] = data_arr[item.Item2];
                data_arr[item.Item2] = Oqf;
            }
            var aa = string.Join(",", data_arr);
            var first2 = aa.IndexOf(",2,");
            var splitstr = aa.Substring(1, first2);
            var d = aa.Split(splitstr).Skip(4).ToArray();
            return d;
        }
        List<(int,int)> MakeStep1(string[] arr, int step1_seed)
        {
            var result= new List<(int, int)>();
            var seed = (uint)step1_seed;
            int vqf;
            int Dqf;
            string Oqf;
            var len = arr.Length;
            for (var index = 0; index < len; index++)
            {
                vqf = (int)(seed >> 8 & 65535) % len;
                seed *= 65793;
                seed &= 4294967295;
                seed += 4282663;
                Dqf = (int)((seed &= 8388607) >> 8 & 65535) % len;
                seed *= 65793;
                seed &= 4294967295;
                seed += 4282663;
                seed &= 8388607;
                //if(!result.ContainsKey(vqf))
                result.Add((vqf, Dqf));
            }

            return result;
        }

        Dictionary<char,List<char>> MakeStep2(int step2_seed)
        {
            var result=new System.Collections.Concurrent.ConcurrentDictionary<char, List<char>>();
            var all_char = " !#$%&()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            Parallel.ForEach(all_char, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, c =>
            {
                var str = "".PadRight(data.Length, c);
                var seed = (uint)step2_seed;
                var r = new List<char>();
                var arr = new long[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 1, -1, 2, 3, 4, 5, -1, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, -1, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91 };

                for (var index = 0; index < str.Length; index++)
                {
                    var current = str[index];
                    var JEp = seed >> 8 & 65535;
                    seed *= 65793;
                    seed &= 4294967295;
                    seed += 4282663;
                    seed &= 8388607;
                    var DEp = arr[str[index]];
                    var nEp = (int)str[index];
                    if (nEp >= 32 && nEp < 127)
                        DEp = arr[nEp];

                    if (DEp >= 0)
                    {
                        DEp += JEp % all_char.Length;
                        DEp %= all_char.Length;
                        current = all_char[(int)DEp];
                        r.Add(current);
                    }
                }
                result.TryAdd(c, r);
            });
           
            return result.ToDictionary(o=>o.Key,v=>v.Value);
        }
    }
}
