namespace AnvilEditor.Helpers
{
    using Models;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    internal static class RenderHelper
    {
        /// <summary>
        /// The current zoom level of the map
        /// </summary>
        internal static int ImageZoom = 2;

        /// <summary>
        /// The current X coordinate of the image on the canvas
        /// </summary>
        internal static double ImageX = 0;

        /// <summary>
        /// The current Y coordinate of the image on the canvas
        /// </summary>
        internal static double ImageY = 0;

        /// <summary>
        /// The currently selected objective, or null if no objective is selected
        /// </summary>
        internal static ObjectiveBase SelectedObjective;

        /// <summary>
        /// Modifies the map image transformation to zoom in or out
        /// </summary>
        /// <param name="zoomIn"></param>
        /// <param name="windowPosition"></param>
        /// <param name="mapPosition"></param>
        internal static void ZoomImage(bool zoomIn, Point windowPosition, Point mapPosition)
        {
            var oldZoom = ImageZoom;
            ImageZoom = zoomIn ? Math.Min(15, ++ImageZoom) : Math.Max(1, --ImageZoom);

            if (oldZoom != ImageZoom)
            {
                ImageX = mapPosition.X;
                ImageY = mapPosition.Y;
            }
        }

        /// <summary>
        /// Draws the given mission on the passed canvas
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="mission"></param>
        /// <param name="objectiveBase"></param>
        internal static void Render(Canvas canvas, Mission mission, ObjectiveBase selectedObjective, MouseButtonEventHandler shapeMouseDownAction)
        {         
            // remove all the old shapes from the grid
            canvas.Children.Clear();

            // draw the mission components
            DrawPrerequisites(canvas, mission, selectedObjective);
            DrawObjectives(canvas, mission, shapeMouseDownAction);
            DrawAmbientZones(canvas, mission, shapeMouseDownAction);
            DrawRespawnMarker(canvas, mission);
            HighlightSelectedObjective(canvas, selectedObjective);
        }

        /// <summary>
        /// Draws prerequisite lines between objectives
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="mission"></param>
        /// <param name="selectedObjective"></param>
        private static void DrawPrerequisites(Canvas canvas, Mission mission, ObjectiveBase selectedObjective)
        {

            // draw all the pre-requisites on
            foreach (var obj in mission.Objectives)
            {
                foreach (var p in obj.Prerequisites)
                {
                    var pre = mission.GetObjective(p);
                    var l = new Line();
                    l.X1 = pre.ScreenX;
                    l.Y1 = pre.ScreenY;
                    l.X2 = obj.ScreenX;
                    l.Y2 = obj.ScreenY;
                    l.Stroke = obj == selectedObjective ? Brushes.Green : Brushes.Black;
                    l.StrokeThickness = RenderHelper.ImageZoom < 5 ? 2 : 1;
                    l.StrokeEndLineCap = PenLineCap.Triangle;
                    canvas.Children.Add(l);
                }
            }
        }

        /// <summary>
        /// Draws objective markers
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="mission"></param>
        /// <param name="shapeMouseDownAction"></param>
        /// <param name="MarkerRadius"></param>
        private static void DrawObjectives(Canvas canvas, Mission mission, MouseButtonEventHandler shapeMouseDownAction)
        {

            // draw all the objective markers in a second pass to make sure they are on top
            foreach (var obj in mission.Objectives)
            {
                if (obj.Ammo)
                {
                    var ammo = BuildRect(canvas, BrushHelper.NewAmmo, obj.ScreenX, obj.ScreenY, MarkerRadius);
                }

                if (obj.Special)
                {
                    var ammo = BuildRect(canvas, BrushHelper.NewSpecial, obj.ScreenX, obj.ScreenY, MarkerRadius);
                }

                if (obj.NewSpawn)
                {
                    var ammo = BuildRect(canvas, BrushHelper.NewSpawn, obj.ScreenX, obj.ScreenY, MarkerRadius);
                }

                var s = BuildRect(canvas, obj.IsOccupied ? BrushHelper.Objective : BrushHelper.UnoccupiedObjective,
                    obj.ScreenX, obj.ScreenY, MarkerRadius, "Objective #" + obj.Id.ToString(), obj.Id);
                s.MouseDown += shapeMouseDownAction;
            }
        }

        /// <summary>
        /// Draws the respawn marker
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="mission"></param>
        /// <param name="MarkerRadius"></param>
        private static void DrawRespawnMarker(Canvas canvas, Mission mission)
        {

            // draw the respawn marker
            if (mission.RespawnX != 0 || mission.RespawnY != 0)
            {
                var rs = BuildRect(canvas, BrushHelper.Respawn, Objective.MapToCanvasX(mission.RespawnX),
                    Objective.MapToCanvasY(mission.RespawnY), MarkerRadius, "Respawn", "respawn_west");
            }
        }

        /// <summary>
        /// Draws the selected objective
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="selectedObjective"></param>
        /// <param name="MarkerRadius"></param>
        private static void HighlightSelectedObjective(Canvas canvas, ObjectiveBase selectedObjective)
        {

            // draw the selected objective
            if (selectedObjective != null)
            {
                var rs = BuildRect(canvas, BrushHelper.Selection, selectedObjective.ScreenX,
                    selectedObjective.ScreenY, MarkerRadius);
            }
        }

        /// <summary>
        /// Draws ambient zones
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="mission"></param>
        /// <param name="shapeMouseDownAction"></param>
        /// <param name="MarkerRadius"></param>
        private static void DrawAmbientZones(Canvas canvas, Mission mission, MouseButtonEventHandler shapeMouseDownAction)
        {

            // draw all the ambient markers
            foreach (var obj in mission.AmbientZones)
            {
                var a = BuildRect(canvas, obj.IsOccupied ? BrushHelper.Ambient : BrushHelper.UnoccupiedAmbient,
                    obj.ScreenX, obj.ScreenY, MarkerRadius, "Ambient #" + obj.Id.ToString(), "A_" + obj.Id.ToString());
                a.MouseDown += shapeMouseDownAction;
            }
        }

        /// <summary>
        /// Generates a rectangle for rendering on the canvas
        /// </summary>
        /// <param name="brush"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="r"></param>
        /// <param name="tooltip"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private static Rectangle BuildRect(Canvas canvas, Brush brush, double x, double y, double r, string tooltip = "", object tag = null)
        {
            var s = new Rectangle();
            s.Fill = brush;
            s.Width = 2 * r;
            s.Height = 2 * r;

            if (tooltip != "")
                s.ToolTip = tooltip;

            if (tag != null)
                s.Tag = tag;

            canvas.Children.Add(s);
            Canvas.SetLeft(s, x - r);
            Canvas.SetTop(s, y - r);

            return s;
        }

        /// <summary>
        /// Returns a marker radius which is dependent on zoom level
        /// </summary>
        private static double MarkerRadius
        {
            get
            {
                return 9 - 0.55 * ImageZoom;
            }
        }
    }
}
