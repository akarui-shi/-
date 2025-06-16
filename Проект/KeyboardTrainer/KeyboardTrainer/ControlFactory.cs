namespace KeyboardTrainer
{ 
    public static class ControlFactory
    {
        public static Button CreateButton(string text, int x, int y, int width, int height, Color backColor, Action<Button>? configure = null)
        {
            var button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12)
            };
            
            button.FlatAppearance.BorderSize = 0;
            configure?.Invoke(button);
            return button;
        }

        public static Label CreateLabel(string text, int x, int y, int? width = null, ContentAlignment align = ContentAlignment.MiddleLeft)
        {
            var label = new Label
            {
                Text = text,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12),
                Location = new Point(x, y),
                AutoSize = width == null
            };
            
            if (width.HasValue) label.Size = new Size(width.Value, label.Height);
            label.TextAlign = align;
            return label;
        }

        public static TextBox CreateTextBox(string text, int x, int y, int width, bool readOnly = false)
        {
            return new TextBox
            {
                Text = text,
                Font = new Font("Segoe UI", 12),
                Location = new Point(x, y),
                Size = new Size(width, 30),
                ReadOnly = readOnly
            };
        }

        public static RichTextBox CreateRichTextBox(int x, int y, int width, int height, bool readOnly = false)
        {
            return new RichTextBox
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(x, y),
                Size = new Size(width, height),
                ReadOnly = readOnly,
                AcceptsTab = false,
                WordWrap = false
            };
        }

        public static ComboBox CreateComboBox(int x, int y, int width, object[]? items = null)
        {
            var combo = new ComboBox
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(x, y),
                Size = new Size(width, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            
            if (items != null) combo.Items.AddRange(items);
            return combo;
        }

        public static ListBox CreateListBox(int x, int y, int width, int height)
        {
            return new ListBox
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(x, y),
                Size = new Size(width, height)
            };
        }

        public static RadioButton CreateRadioButton(string text, int x, int y, bool checkedState = false)
        {
            return new RadioButton
            {
                Text = text,
                Location = new Point(x, y),
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.White,
                Checked = checkedState,
                Width = 600
            };
        }
    }
}