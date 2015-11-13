using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Note
{
    // Filler
}

public class Hero : MonoBehaviour {
    public float speed = 4f;
	public float floatingFreq = 1.65f;
	public float floatingAmp = 0.1f;
	public float turningSpeed = 0.3f;

    public void MoveTo(Vector2 dest)
    {
        this.commandQ.Enqueue(new HeroCommand(dest));
    }

    public void PickUpMinion(Vector2 dest, Minion minion)
    {
        this.commandQ.Enqueue(new HeroCommand(dest, pickUp: minion));
    }

    public void TurnInNote(Vector2 dest, Note note)
    {
        this.commandQ.Enqueue(new HeroCommand(dest, turnIn: note));
    }

    /****************************** PRIVATE ******************************/

    private class HeroCommand
    {
        // <x, y> vectors representing the start and finish positions
        // for this command
        public Vector2 start, finish;

        // If a minion should be picked up when the command is finished,
        // pickUp is used. Otherwise, pickUp is null.
        public Minion pickUp;

        // If a minion should be placed on a note when the command is finished,
        // turnIn is used. Otherwise, turnIn is null.
        public Note turnIn;

        public HeroCommand(Vector2 destination, Minion pickUp = null, Note turnIn = null)
        {
            this.finish = destination;
            this.pickUp = pickUp;
            this.turnIn = turnIn;
        }
    }

    // Queue of commands to execute next
    private Queue<HeroCommand> commandQ;

    // Command currently being executed
    private HeroCommand currentCommand;

    // Whether the hero is waiting for a new command (floating) or
    // currently executing a command (flying)
    private enum HeroState
    {
        FLOATING,
        FLYING
    };
    private HeroState state;

    // During FLYING state, keep track of current and total time
    private float currentMoveTime, totalMoveTime;

    // During FLOATING state, keep track of time elapsed
    private float floatingTime;

	private float currentScale, destScale;
	private bool turning;

	// Equilibrium position. center of floating wave
	private Vector2 eqPos;

	// List of minions currently being carried
	private LinkedList<Minion> minionsCarrying;

	private Vector2 floatingDelta {
		get {
			return new Vector2(
				0f,
				this.floatingAmp * Mathf.Sin(this.floatingTime * this.floatingFreq)
			);
		}
	}

	private Vector2 position {
		get {
			return this.eqPos + this.floatingDelta;
		}
		set {
			this.eqPos = value - this.floatingDelta;
		}
	}

	protected void Start()
	{
		this.commandQ = new Queue<HeroCommand>();
		this.state = HeroState.FLOATING;
		this.floatingTime = 0f;
		this.eqPos = this.transform.position;
		this.currentScale = 1f;
		this.destScale = 1f;
		this.turning = false;

		this.minionsCarrying = new LinkedList<Minion> ();
	}
	
	/** Calculate total flight to get from start to finish
     */
    private float FlightTime(Vector2 start, Vector2 finish)
    {
        float dist = Vector2.Distance(start, finish);
        return Mathf.Sqrt(dist) / this.speed;
    }

    private void BeginCommand(HeroCommand cmd)
    {
        cmd.start = transform.position;

		this.currentScale = this.transform.localScale.x;
		this.destScale = 
			(cmd.start.x < cmd.finish.x) ? 1f :
				(cmd.start.x > cmd.finish.x) ? -1f : this.currentScale;
		this.turning = true;
		
        this.totalMoveTime = this.FlightTime(cmd.start, cmd.finish);
        this.currentMoveTime = 0f;
        this.state = HeroState.FLYING;
        this.currentCommand = cmd;
    }

    private void FinishCommand()
    {
        HeroCommand cmd = this.currentCommand;

		this.floatingTime = (cmd.finish.y > cmd.start.y) ? 0 : (Mathf.PI / this.floatingFreq);
		this.eqPos = cmd.finish;

        if (cmd.pickUp != null)
        {
			this.minionsCarrying.AddLast(new LinkedListNode<Minion>(this.currentCommand.pickUp));
        }
        if (cmd.turnIn != null)
        {
            // TODO
        }

        if (this.commandQ.Count > 0)
        {
            this.BeginCommand(this.commandQ.Dequeue());
        } else
        {
            this.state = HeroState.FLOATING;
            this.currentCommand = null;
        }
    }

	private void UpdateScale() 
	{
		if (!this.turning)
			return;

		if (this.currentScale > this.destScale) {
			this.currentScale -= this.turningSpeed;
			if (this.currentScale <= this.destScale) {
				this.currentScale = this.destScale;
				this.turning = false;
			}
		} else if (this.currentScale < this.destScale) {
			this.currentScale += this.turningSpeed;
			if (this.currentScale >= this.destScale) {
				this.currentScale = this.destScale;
				this.turning = false;
			}
		} else {
			this.turning = false;
		}

		this.transform.localScale = new Vector3 (this.currentScale, 1, 1);
	}

    private void SetFloatingTransform()
    {
		this.transform.position = this.position;
		this.UpdateScale ();
    }

    private void SetFlyingTransform()
    {
        HeroCommand cmd = this.currentCommand;

        // What portion of the way are we through our flight command?
        float t = (this.totalMoveTime == 0f) ? 1f : this.currentMoveTime / this.totalMoveTime;

        // "Smootherstep"
        // https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
        t = t * t * t * (t * (6f * t - 15f) + 10f);

        this.transform.position = Vector2.Lerp(cmd.start, cmd.finish, t);
		this.UpdateScale ();
    }

    protected void FixedUpdate () {
        if (this.state == HeroState.FLOATING)
        {
            this.floatingTime = (this.floatingTime + Time.fixedDeltaTime) % (2f * Mathf.PI / this.floatingFreq);
			if (this.commandQ.Count > 0)
            	this.BeginCommand(this.commandQ.Dequeue());
        } else if (this.state == HeroState.FLYING)
        {
            this.currentMoveTime += Time.fixedDeltaTime;
            if (this.currentMoveTime >= this.totalMoveTime)
            {
                this.FinishCommand();
            }
        }

        if (this.state == HeroState.FLOATING)
        {
            this.SetFloatingTransform();
        } else if (this.state == HeroState.FLYING)
        {
            this.SetFlyingTransform();
        }
	}
}
