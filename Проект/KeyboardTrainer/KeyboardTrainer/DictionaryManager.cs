using System.Text;

namespace KeyboardTrainer
{
    public class DictionaryManager
    {
        private readonly string _dictionariesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dictionaries");

        public DictionaryManager() => EnsureDictionariesFolderExists();

        private void EnsureDictionariesFolderExists()
        {
            if (!Directory.Exists(_dictionariesFolder))
                Directory.CreateDirectory(_dictionariesFolder);
        }

        public List<string> GetDictionaries() =>
            Directory.GetFiles(_dictionariesFolder, "*.txt")
                .Select(file => Path.GetFileNameWithoutExtension(file)!)
                .ToList();

        public List<string> GetContent(string dictionaryName, bool isSentenceDictionary)
        {
            string filePath = GetDictionaryPath(dictionaryName);
            if (!File.Exists(filePath)) return new List<string>();

            var content = File.ReadAllLines(filePath, Encoding.UTF8)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            if (isSentenceDictionary)
            {
                // Для словарей с предложениями объединяем строки, разделенные точками
                var sentences = new List<string>();
                StringBuilder currentSentence = new StringBuilder();

                foreach (var line in content)
                {
                    var parts = line.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        var trimmed = part.Trim();
                        if (!string.IsNullOrEmpty(trimmed))
                        {
                            currentSentence.Append(trimmed);
                            sentences.Add(currentSentence.ToString());
                            currentSentence.Clear();
                        }
                    }
                }

                return sentences;
            }

            return content;
        }

        public void SaveDictionary(string name, IEnumerable<string> content, bool isSentenceDictionary) =>
            File.WriteAllLines(GetDictionaryPath(name), content, Encoding.UTF8);

        public void DeleteDictionary(string? name)
        {
            string filePath = GetDictionaryPath(name);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        private string GetDictionaryPath(string? dictionaryName) =>
            Path.Combine(_dictionariesFolder, $"{dictionaryName}.txt");
    }
}