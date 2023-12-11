using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class CameraScript : Node3D
{
    public Camera3D MainCamera;

    private float cameraZoom = 70;
    private float cameraZoomTo = 70;
    private int cameraZoomStrenght = 10;
    private int cameraZoomMax = 100;
    private int cameraZoomMin = 20;
    private float smoothTime = 0.2F;

    private float cameraMoveSpeed = 0.5f;
    private Vector2 mousePosition;
    private int winWidth;
    private int winHeight;
    public override void _Ready()
	{
        MainCamera = GetNode<Camera3D>("MainCamera");
        winWidth = (int)GetViewport().GetVisibleRect().Size.X;
        winHeight = (int)GetViewport().GetVisibleRect().Size.Y;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
        #region CameraZoom
        
        cameraZoom = Mathf.Lerp(cameraZoom, cameraZoomTo, smoothTime);
        MainCamera.Fov = cameraZoom;

        if (Input.IsActionJustReleased("scroll_up"))

        {
            cameraZoomTo = Mathf.Clamp(cameraZoomTo - cameraZoomStrenght, cameraZoomMin, cameraZoomMax);
		}

        else if (Input.IsActionJustReleased("scroll_down"))
        {
            cameraZoomTo = Mathf.Clamp(cameraZoomTo + cameraZoomStrenght, cameraZoomMin, cameraZoomMax);
        }
        #endregion
        #region CameraKeyMovement
        mousePosition = GetViewport().GetMousePosition();
        if (Input.IsActionPressed("left") || mousePosition.X <= winWidth * 0.02)
        {
            MainCamera.Translate(Vector3.Left * cameraMoveSpeed);
        }
        if (Input.IsActionPressed("right") || mousePosition.X >= winWidth * 0.98)
        {
            MainCamera.Translate(Vector3.Right * cameraMoveSpeed);
        }
        if (Input.IsActionPressed("up") || mousePosition.Y <= winHeight * 0.02)
        {
            MainCamera.Translate(Vector3.Up * cameraMoveSpeed);
        }
        if (Input.IsActionPressed("down") || mousePosition.Y >= winHeight * 0.98)
        {
            MainCamera.Translate(Vector3.Down * cameraMoveSpeed);
        }
        #endregion
    }
}
