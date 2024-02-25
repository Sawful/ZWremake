using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class CameraScript : Camera3D
{

    private float cameraZoom = 10;
    private float cameraZoomTo = 10;
    private int cameraZoomStrength = 5;
    private int cameraZoomMax = 15;
    private int cameraZoomMin = 5;
    private float smoothTime = 0.1F;

    private float cameraMoveSpeed = 0.5f;
    private Vector2 mousePosition;
    private int winWidth;
    private int winHeight;
    public override void _Ready()
	{
        winWidth = (int)GetViewport().GetVisibleRect().Size.X;
        winHeight = (int)GetViewport().GetVisibleRect().Size.Y;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
        #region CameraZoom
        
        cameraZoom = Mathf.Lerp(cameraZoom, cameraZoomTo, smoothTime);
        Position = Vector3.Right * Position.X + Vector3.Back * Position.Z + Vector3.Up * cameraZoom;

        if (Input.IsActionJustReleased("ScrollUp"))
        {
            cameraZoomTo = Mathf.Clamp(cameraZoomTo - cameraZoomStrength, cameraZoomMin, cameraZoomMax);
		}

        else if (Input.IsActionJustReleased("ScrollDown"))
        {
            cameraZoomTo = Mathf.Clamp(cameraZoomTo + cameraZoomStrength, cameraZoomMin, cameraZoomMax);
        }

        #endregion

        #region CameraKeyMovement
        mousePosition = GetViewport().GetMousePosition();
        if (Input.IsActionPressed("Left") || mousePosition.X <= winWidth * 0.02)
        {
            Translate(Vector3.Left * cameraMoveSpeed);
        }
        if (Input.IsActionPressed("Right") || mousePosition.X >= winWidth * 0.98)
        {
            Translate(Vector3.Right * cameraMoveSpeed);
        }
        if (Input.IsActionPressed("Up") || mousePosition.Y <= winHeight * 0.02)
        {
            Translate(Vector3.Up * cameraMoveSpeed);
        }
        if (Input.IsActionPressed("Down") || mousePosition.Y >= winHeight * 0.98)
        {
            Translate(Vector3.Down * cameraMoveSpeed);
        }
        #endregion
    }
}
