namespace OS_LW8;
using Microsoft.Win32;
public partial class MainForm : Form
{
    TreeView registry_counter_tree;
    DataGridView key_datagridview;
    MenuStrip menu_bar;
    ToolStripMenuItem file;

    ToolStripMenuItem create;
    ToolStripMenuItem add;
    ToolStripMenuItem delete_value;
    ToolStripMenuItem delete_directory;
    ToolStripMenuItem rewrite;

    ToolStripMenuItem load_and_save;
    ToolStripMenuItem load;
    public MainForm()
    {
        this.Size = new Size(700, 500);
        this.Text = "Реестр";

        menu_bar = new MenuStrip();
        file = new ToolStripMenuItem("Файл");

        create = new ToolStripMenuItem("Создать ключ");
        create.ToolTipText = "Создать ключ в разделе программы";
        create.Click += new EventHandler(create_Click);
        file.DropDownItems.Add(create);

        add = new ToolStripMenuItem("Добавить значение");
        add.ToolTipText = "Добавить новое значение в ключ раздела программы";
        add.Click += new EventHandler(add_Click);
        file.DropDownItems.Add(add);

        delete_value = new ToolStripMenuItem("Удалить значение");
        delete_value.ToolTipText = "Удалить уже существующее значение";
        delete_value.Click += new EventHandler(delete_value_Click);
        file.DropDownItems.Add(delete_value);

        rewrite = new ToolStripMenuItem("Перезаписать");
        rewrite.ToolTipText = "Перезаписать значнеия из таблицы";
        rewrite.Click += new EventHandler(rewrite_Click);
        file.DropDownItems.Add(rewrite);

        menu_bar.Items.Add(file);

        load_and_save = new ToolStripMenuItem("Загрузка и сохранение");

        load = new ToolStripMenuItem("Загрузить");
        load.ToolTipText = "Загрузать .ini файл";
        load.Click += new EventHandler(load_Click);
        load_and_save.DropDownItems.Add(load);

        menu_bar.Items.Add(load_and_save);

        this.Controls.Add(menu_bar);

        registry_counter_tree = new TreeView();
        registry_counter_tree.Location = new Point(15, 25);
        registry_counter_tree.Size = new Size((int)(this.Width * 0.25), this.Height - 75);
        this.Controls.Add(registry_counter_tree);

        this.registry_counter_tree.Nodes.Add(Registry.CurrentUser.Name, Registry.CurrentUser.Name);
        this.registry_counter_tree.Nodes[Registry.CurrentUser.Name].Nodes.Add(string.Empty);
        this.registry_counter_tree.Nodes[Registry.CurrentUser.Name].Tag = Registry.CurrentUser;

        key_datagridview = new DataGridView();
        key_datagridview.Location = new Point(registry_counter_tree.Location.X + registry_counter_tree.Size.Width + 15, registry_counter_tree.Location.Y);
        key_datagridview.Size = new Size(this.Width - registry_counter_tree.Width - 65, registry_counter_tree.Height);
        key_datagridview.ColumnCount = 2;
        key_datagridview.Columns[0].HeaderCell.Value = "Name";
        key_datagridview.Columns[1].HeaderCell.Value = "Value";
        key_datagridview.RowHeadersVisible = false;
        key_datagridview.Columns[0].Width = (int)(key_datagridview.Width / 3);
        key_datagridview.Columns[1].Width = key_datagridview.Width - key_datagridview.Columns[0].Width - 3;
        this.Controls.Add(key_datagridview);

        this.registry_counter_tree.NodeMouseClick += new TreeNodeMouseClickEventHandler(registry_counter_tree_NodeMouseClick);
        this.registry_counter_tree.BeforeExpand += new TreeViewCancelEventHandler(registry_counter_tree_BeforeExpand);
    }

    void load_Click(object sender, EventArgs e)
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true).OpenSubKey("RegustryEditor", true) as RegistryKey;
        OpenFileDialog file_dialog = new OpenFileDialog();
        file_dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        file_dialog.Filter = "ini files (*.ini)|*.ini";
        if (file_dialog.ShowDialog() != DialogResult.OK) { key.Close(); return; }
        string[] lines = File.ReadAllLines(file_dialog.FileName);
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == "[key]")
            {
                if (lines[i + 1] == null || !(lines[i + 1].Contains("name="))) { return; }
                i++;
                key.CreateSubKey(lines[i].Substring(5));
            }
            else if (lines[i] == "[value]")
            {
                if (lines[i + 1] == null || !(lines[i + 1].Contains("name="))) { return; }
                string name = lines[i+1].Substring(5);
                if (lines[i + 2] == null || !(lines[i + 2].Contains("value="))) { return; }
                i+=2;
                key.SetValue(name, lines[i].Substring(6));
            }
        }
        registry_counter_tree.Refresh();
        refresh_table();
    }

    void delete_value_Click(object sender, EventArgs e)
    {
        RegistryKey software_key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true) as RegistryKey;
        if (!(software_key.GetSubKeyNames().Contains<string>("RegustryEditor")))
        {
            MessageBox.Show("Директория не существует!", "Ошибка удаления значения", MessageBoxButtons.OK);
            return;
        }
        RegistryKey key = software_key.OpenSubKey("RegustryEditor", true) as RegistryKey;
        software_key.Close();
        string name = Microsoft.VisualBasic.Interaction.InputBox("Введите название значения", "Ввод названия");
        if (key.GetValue(name) == null)
        {
            MessageBox.Show("Значение не существует!", "Ошибка удаления значения", MessageBoxButtons.OK);
            key.Close();
            return;
        }
        key.DeleteValue(name);
        key.Close();
        registry_counter_tree.Refresh();
        refresh_table();
    }

    void add_Click(object sender, EventArgs e)
    {
        RegistryKey software_key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true) as RegistryKey;
        if (!(software_key.GetSubKeyNames().Contains<string>("RegustryEditor")))
        {
            MessageBox.Show("Директория не существует!", "Ошибка добавления значения", MessageBoxButtons.OK);
            return;
        }
        RegistryKey key = software_key.OpenSubKey("RegustryEditor", true) as RegistryKey;
        software_key.Close();
        string new_name = Microsoft.VisualBasic.Interaction.InputBox("Введите название значения", "Ввод названия");
        string new_value = Microsoft.VisualBasic.Interaction.InputBox("Введите значение", "Ввод значения");
        key.SetValue(new_name, new_value);
        key.Close();
        registry_counter_tree.Refresh();
        refresh_table();
    }

    void create_Click(object sender, EventArgs e)
    {
        RegistryKey software_key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true) as RegistryKey;
        if (software_key.GetSubKeyNames().Contains<string>("RegustryEditor"))
        {
            MessageBox.Show("Директория уже существует!", "Ошибка создания директории", MessageBoxButtons.OK);
            return;
        }
        RegistryKey new_key = software_key.CreateSubKey("RegustryEditor", true);
        software_key.Close();
        string new_value = Microsoft.VisualBasic.Interaction.InputBox("Введите значение", "Ввод значения");
        new_key.Close();
        registry_counter_tree.Refresh();
    }

    void rewrite_Click(object sender, EventArgs e)
    {
        RegistryKey software_key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true) as RegistryKey;
        if (!(software_key.GetSubKeyNames().Contains<string>("RegustryEditor")))
        {
            MessageBox.Show("Директория не существует!", "Ошибка добавления значения", MessageBoxButtons.OK);
            return;
        }
        RegistryKey key = software_key.OpenSubKey("RegustryEditor", true) as RegistryKey;
        software_key.Close();
        string name = Microsoft.VisualBasic.Interaction.InputBox("Введите название значения", "Ввод названия");
        if (key.GetValue(name) == null)
        {
            MessageBox.Show("Значение не существует!", "Ошибка удаления значения", MessageBoxButtons.OK);
            key.Close();
            return;
        }
        string new_value = Microsoft.VisualBasic.Interaction.InputBox("Введите значение", "Ввод значения");
        key.SetValue(name, new_value);
        key.Close();
        registry_counter_tree.Refresh();
        refresh_table();
    }

    void registry_counter_tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
        RegistryKey key = e.Node.Tag as RegistryKey;
        if (key.ValueCount == 0)
        {
            key_datagridview.RowCount = 1;

            key_datagridview[0, 0].Value = "(Default)";
            key_datagridview[1, 0].Value = "(No value)";
            key_datagridview.Enabled = false;
            return;
        }
        if (key_datagridview.Enabled == false) { key_datagridview.Enabled = true; }
        if (key != null)
        {
            int i = 0;
            key_datagridview.RowCount = key.ValueCount;
            foreach (string name in key.GetValueNames())
            {
                key_datagridview[0, i].Value = name;
                key_datagridview[1, i].Value = key.GetValue(name);
                key_datagridview[1, i].Tag = key;
                i++;
            }
        }
    }

    void refresh_table()
    {
        RegistryKey key = registry_counter_tree.SelectedNode.Tag as RegistryKey;
        if (key.ValueCount == 0)
        {
            key_datagridview.RowCount = 1;

            key_datagridview[0, 0].Value = "(Default)";
            key_datagridview[1, 0].Value = "(No value)";
            key_datagridview.Enabled = false;
            return;
        }
        if (key_datagridview.Enabled == false) { key_datagridview.Enabled = true; }
        if (key != null)
        {
            int i = 0;
            key_datagridview.RowCount = key.ValueCount;
            foreach (string name in key.GetValueNames())
            {
                key_datagridview[0, i].Value = name;
                key_datagridview[1, i].Value = key.GetValue(name);
                key_datagridview[1, i].Tag = key;
                i++;
            }
        }
    }

    void registry_counter_tree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
    {
        if (e.Node.Nodes.Count != 1 || e.Node.Nodes[0].Text != string.Empty) { return; }
        e.Node.Nodes.Clear();
        RegistryKey key = e.Node.Tag as RegistryKey;
        if (key == null) { return; }
        foreach (string name in key.GetSubKeyNames())
        {
            e.Node.Nodes.Add(name, name);
            if (name == "SECURITY" || name == "SAM") { continue; }
            RegistryKey subkey = key.OpenSubKey(name);
            e.Node.Nodes[name].Tag = subkey;
            if (subkey.SubKeyCount > 0)
                e.Node.Nodes[name].Nodes.Add(string.Empty);
        }
    }
}
