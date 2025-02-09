using Godot;

public partial class Hud : CanvasLayer
{
	[Signal]
	public delegate void StartGameEventHandler();

    private Label			_message;
	private Label			_scoreLabel;
	private Timer			_messageTimer;
	private Button			_startButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_message = GetNode<Label>("Message");
		_scoreLabel = GetNode<Label>("ScoreLabel");
		_messageTimer = GetNode<Timer>("MessageTimer");
		_startButton = GetNode<Button>("StartButton");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	public void ShowMessage(string text)
	{
		_message.Text = text;
		_message.Show();
		_messageTimer.Start();
	}

	async public void ShowGameOver()
	{
		// Shows game over message for x-seconds set in MessageTimer inspector
		ShowMessage("Game Over");
        await ToSignal(_messageTimer, Timer.SignalName.Timeout);

		// Then return to title screen and after a 1-second pause show start button
		_message.Text = "Dodge the Creeps!";
		_message.Show();
		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
        _startButton.Show();
	}

	public void UpdateScore(int score)
	{
        _scoreLabel.Text = score.ToString();
	}

	private void OnStartButtonPressed()
	{
        _startButton.Hide();
        EmitSignal(SignalName.StartGame);
	}

	private void OnMessageTimerTimeout()
	{
		_message.Hide();
	}
}