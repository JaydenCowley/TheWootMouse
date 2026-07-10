namespace WootMouseSettingsUI;

partial class MainUI
{
    private System.ComponentModel.IContainer components = null!;

    // Layout
    private TableLayoutPanel rootLayout = null!;
    private GroupBox keysGroup = null!;
    private GroupBox mouseGroup = null!;
    private GroupBox previewGroup = null!;
    private FlowLayoutPanel buttonsPanel = null!;

    // Key bindings
    private Label keyUpLabel = null!;          private ComboBox keyUpCombo = null!;
    private Label keyDownLabel = null!;        private ComboBox keyDownCombo = null!;
    private Label keyLeftLabel = null!;        private ComboBox keyLeftCombo = null!;
    private Label keyRightLabel = null!;       private ComboBox keyRightCombo = null!;
    private Label keyScrollUpLabel = null!;    private ComboBox keyScrollUpCombo = null!;
    private Label keyScrollDownLabel = null!;  private ComboBox keyScrollDownCombo = null!;
    private Label keyTurboLabel = null!;       private ComboBox keyTurboCombo = null!;
    private Label mouseLayerKeyLabel = null!;  private ComboBox mouseLayerKeyCombo = null!;
    private CheckBox mouseLayerEnabledCheck = null!;

    // Numeric / curve settings
    private Label deadzoneLabel = null!;        private TrackBar deadzoneTrack = null!;        private Label deadzoneValueLabel = null!;
    private Label maxSpeedLabel = null!;        private NumericUpDown maxSpeedNumeric = null!;
    private Label scrollSpeedLabel = null!;     private NumericUpDown scrollSpeedNumeric = null!;
    private Label turboMultiplierLabel = null!; private NumericUpDown turboMultiplierNumeric = null!;
    private Label curvePowerLabel = null!;      private NumericUpDown curvePowerNumeric = null!;

    // Buttons + status
    private Button saveButton = null!;
    private Button reloadButton = null!;
    private Label statusLabel = null!;
    private Label sdkStatusLabel = null!;

    // Analog preview
    private AnalogStickDisplay analogDisplay = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        // --- Form ---
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 620);
        Text = "WootMouse Settings";
        MinimumSize = new Size(820, 600);

        // --- Root layout: 2 columns ---
        rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(10),
        };
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // --- Left column: keys + mouse settings stacked ---
        var leftStack = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
        };
        leftStack.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
        leftStack.RowStyles.Add(new RowStyle(SizeType.Percent, 40));

        // === Keys group ===
        keysGroup = new GroupBox { Text = "Key Bindings", Dock = DockStyle.Fill, Padding = new Padding(8) };
        var keysLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 9,
        };
        keysLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        keysLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        for (int i = 0; i < 9; i++) keysLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        AddKeyRow(keysLayout, 0, "Move Up",       out keyUpLabel,         out keyUpCombo);
        AddKeyRow(keysLayout, 1, "Move Down",     out keyDownLabel,       out keyDownCombo);
        AddKeyRow(keysLayout, 2, "Move Left",     out keyLeftLabel,       out keyLeftCombo);
        AddKeyRow(keysLayout, 3, "Move Right",    out keyRightLabel,      out keyRightCombo);
        AddKeyRow(keysLayout, 4, "Scroll Up",     out keyScrollUpLabel,   out keyScrollUpCombo);
        AddKeyRow(keysLayout, 5, "Scroll Down",   out keyScrollDownLabel, out keyScrollDownCombo);
        AddKeyRow(keysLayout, 6, "Turbo",         out keyTurboLabel,      out keyTurboCombo);
        AddKeyRow(keysLayout, 7, "Mouse Layer Key", out mouseLayerKeyLabel, out mouseLayerKeyCombo);

        mouseLayerEnabledCheck = new CheckBox
        {
            Text = "Require mouse-layer key to be held",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(3, 6, 3, 3),
        };
        keysLayout.Controls.Add(mouseLayerEnabledCheck, 0, 8);
        keysLayout.SetColumnSpan(mouseLayerEnabledCheck, 2);

        keysGroup.Controls.Add(keysLayout);
        leftStack.Controls.Add(keysGroup, 0, 0);

        // === Mouse settings group ===
        mouseGroup = new GroupBox { Text = "Mouse Behavior", Dock = DockStyle.Fill, Padding = new Padding(8) };
        var mouseLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 5,
        };
        mouseLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        mouseLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        mouseLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
        for (int i = 0; i < 5; i++) mouseLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Deadzone (trackbar 0..100 -> 0.00..1.00)
        deadzoneLabel = new Label { Text = "Deadzone", Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(3, 8, 3, 3) };
        deadzoneTrack = new TrackBar
        {
            Minimum = 0, Maximum = 100, TickFrequency = 10, SmallChange = 1, LargeChange = 5,
            Dock = DockStyle.Fill,
        };
        deadzoneTrack.Scroll += deadzoneTrack_Scroll;
        deadzoneValueLabel = new Label { Text = "0.10", Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(3, 8, 3, 3) };
        mouseLayout.Controls.Add(deadzoneLabel, 0, 0);
        mouseLayout.Controls.Add(deadzoneTrack, 1, 0);
        mouseLayout.Controls.Add(deadzoneValueLabel, 2, 0);

        AddNumericRow(mouseLayout, 1, "Max Speed (px/s)",  out maxSpeedLabel,        out maxSpeedNumeric,        50,   10_000, 1400);
        AddNumericRow(mouseLayout, 2, "Scroll Speed",      out scrollSpeedLabel,     out scrollSpeedNumeric,      1,      500,   20);
        AddNumericRow(mouseLayout, 3, "Turbo Multiplier",  out turboMultiplierLabel, out turboMultiplierNumeric,  1,       20,    2);
        AddNumericRow(mouseLayout, 4, "Curve Power",       out curvePowerLabel,      out curvePowerNumeric,       1,        8,    2);

        mouseGroup.Controls.Add(mouseLayout);
        leftStack.Controls.Add(mouseGroup, 0, 1);

        rootLayout.Controls.Add(leftStack, 0, 0);

        // === Right column: analog preview ===
        previewGroup = new GroupBox { Text = "Analog Preview", Dock = DockStyle.Fill, Padding = new Padding(8) };
        analogDisplay = new AnalogStickDisplay { Dock = DockStyle.Fill };
        previewGroup.Controls.Add(analogDisplay);
        rootLayout.Controls.Add(previewGroup, 1, 0);

        // === Bottom: buttons + status ===
        buttonsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            WrapContents = false,
            Padding = new Padding(0, 8, 0, 0),
        };
        saveButton = new Button { Text = "Save", AutoSize = true, Padding = new Padding(8, 2, 8, 2) };
        saveButton.Click += saveButton_Click;
        reloadButton = new Button { Text = "Reload", AutoSize = true, Padding = new Padding(8, 2, 8, 2) };
        reloadButton.Click += reloadButton_Click;
        statusLabel = new Label { AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(10, 8, 3, 3) };
        sdkStatusLabel = new Label { AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(10, 8, 3, 3) };

        buttonsPanel.Controls.Add(saveButton);
        buttonsPanel.Controls.Add(reloadButton);
        buttonsPanel.Controls.Add(statusLabel);
        buttonsPanel.Controls.Add(sdkStatusLabel);

        rootLayout.Controls.Add(buttonsPanel, 0, 1);
        rootLayout.SetColumnSpan(buttonsPanel, 2);

        Controls.Add(rootLayout);
    }

    private static void AddKeyRow(TableLayoutPanel parent, int row, string text, out Label label, out ComboBox combo)
    {
        label = new Label
        {
            Text = text, Anchor = AnchorStyles.Left, AutoSize = true,
            Margin = new Padding(3, 8, 3, 3),
        };
        combo = new ComboBox
        {
            Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList,
            Margin = new Padding(3, 4, 3, 4),
        };
        parent.Controls.Add(label, 0, row);
        parent.Controls.Add(combo, 1, row);
    }

    private static void AddNumericRow(TableLayoutPanel parent, int row, string text,
        out Label label, out NumericUpDown numeric, int min, int max, int value)
    {
        label = new Label
        {
            Text = text, Anchor = AnchorStyles.Left, AutoSize = true,
            Margin = new Padding(3, 8, 3, 3),
        };
        numeric = new NumericUpDown
        {
            Minimum = min, Maximum = max, Value = value,
            Dock = DockStyle.Fill, Margin = new Padding(3, 4, 3, 4),
        };
        parent.Controls.Add(label, 0, row);
        parent.Controls.Add(numeric, 1, row);
        parent.SetColumnSpan(numeric, 2);
    }

    #endregion
}