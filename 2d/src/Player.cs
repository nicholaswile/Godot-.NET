using Godot;

public partial class Player : Area2D
{
	[Signal]
	public delegate void HitEventHandler();
	
	// This is basically Unity's SerializeField (shows in inspector)
	[Export]
	public int Speed {get; set;} = 400;
	public Vector2 ScreenSize;
	
	private AnimatedSprite2D		_animatedSprite2D;
	private CollisionShape2D		_collisionShape2D;
	
	// This is basically Unity's Start()
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");

		// Starts player at screen center
		Position = new Vector2(ScreenSize.X/2, ScreenSize.Y/2);
		
		// Hides player when game starts 
		Hide();
		
		Start(Position);
	}
	
	public void Start(Vector2 position) 
	{
		Position = position;
		Show();
		
		_collisionShape2D.Disabled = false; 
	}

	// This is basically Unity's Update()
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Check input
		Vector2 velocity = CheckInput();
		
		// Set animation
		PlayAnimation(velocity);
		
		// Move player
		Move(velocity, (float)delta);
	}
	
	// Detect player input and set velocity
	private Vector2 CheckInput() 
	{
		Vector2 velocity = Vector2.Zero;
		
		if (Input.IsActionPressed("move_right")) {
			velocity.X += 1;
		}
		
		if (Input.IsActionPressed("move_left")) {
			velocity.X -= 1;
		}
		
		if (Input.IsActionPressed("move_up")) {
			velocity.Y -= 1;
		}
		
		if (Input.IsActionPressed("move_down")) {
			velocity.Y += 1;
		}
		
		return velocity;
	}
	
	// Play animation if player is moving
	private void PlayAnimation(Vector2 velocity) 
	{
		if (velocity.Length() <= 0) {
			_animatedSprite2D.Stop();
			return;
		}

		if (velocity.X != 0) {
			_animatedSprite2D.Animation = "walk";
			_animatedSprite2D.FlipV = false;

			// If going left, reverse sprite horizontally
			_animatedSprite2D.FlipH = (velocity.X < 0);
		}
		
		else if (velocity.Y != 0) {
			_animatedSprite2D.Animation = "up";
			_animatedSprite2D.FlipV = (velocity.Y > 0);
		}
		
		_animatedSprite2D.Play();
	}
	
	// Moves player position based on velocity
	private void Move(Vector2 velocity, float delta) 
	{
		velocity = velocity.Normalized() * Speed;
		Position += velocity * delta;
		Position = new Vector2(
			// Prevents player from going out of bounds
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);
	}
	
	// Handle collisions
	private void OnBodyEntered(Node2D body) 
	{
		Hide(); // player disappears after bein hit.
		EmitSignal(SignalName.Hit);

		// Disable player collision after enemy hits so won't trigger after player is defeated.
		_collisionShape2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}
}