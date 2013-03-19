using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace OrbitSim
{
    partial class SimPanel : Control
    {
        public static Space space {get; private set;}
        private static Thread thread;

        public static bool quit { get; set; }

        private static float fps = 30;
        private static long frameTime = (long)(1.0 / fps * 1000);

        private System.Windows.Forms.Timer timer;


        private int centerX; //center x of the simulation (px)
        private int centerY; //center y of the simulation (px)

        private const double positionFactor = 1E-6 * 1E-3;
        private double starRadiusScale; //1px = 10 000 km = 10 000 000 m
        private double bodyRadiusScale; //1px = 1000 km = 1 000 000 m
        private double positionScale; //1px = 1 000 000 km = 1 000 000 000 m
        private double mouseScale; //1px = 500 m
        private double _scale;
        public double scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                bodyRadiusScale = _scale * 1E-6;
                starRadiusScale = realScaleMode ? bodyRadiusScale : bodyRadiusScale * 1E-2;
                positionScale = realScaleMode ? bodyRadiusScale : bodyRadiusScale * 1E-3;
                mouseScale = 500;
                space.setCollisionScales(bodyRadiusScale, starRadiusScale, positionScale);
            }
        }

        public bool drawSpeedVector { get; set; } //True si le simulateur doit dessiner le vecteur vitesse du corps en construction
        public bool drawSpeedInfo { get; set; } //True si le simulateur doit afficher la vitesse des corps en temps réel
        //private bool randomColorMode { get; set; } //True si la couleur du corps en construction est aléatoire
        private bool _realScaleMode;
        public bool realScaleMode //True si tout est à l'échelle (Sans tricher)
        {
            get { return _realScaleMode; }
            set
            {
                _realScaleMode = value;
                this.scale = scale;
            }
        }

        public const double DEFAULT_MASS = 1E24; //en kg
        public const int DEFAULT_RADIUS = 1000000; //en m
        public static Color DEFAULT_COLOR = Color.Blue;
        public const double DEFAULT_SCALE = 1.0;

        //Body in build properties
        public double mass { get; set; } //(kg)
        public int radius { get; set; } //(m)
        private Color color;
        public bool star { get; set; }

        //Stuff needed for the draw of the speed vector during mouse movement
        private int mouseX;
        private int mouseY;
        private int draggedX;
        private int draggedY;
        private Vector2D drawSpeed; //Speed vector to draw of the in build body
        private Vector2D speed; //Spee vector of the inbuild body (m/s)

        public SimPanel(int Width, int Height)
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.PreviewKeyDown += new PreviewKeyDownEventHandler(SimPanel_PreviewKeyDown);
            this.KeyDown += new KeyEventHandler(SimPanel_KeyDown);
            this.MouseClick += new MouseEventHandler(SimPanel_MouseClick);
            this.MouseDown += new MouseEventHandler(SimPanel_MouseDown);
            this.MouseUp += new MouseEventHandler(SimPanel_MouseUp);
            this.MouseMove += new MouseEventHandler(SimPanel_MouseMove);
            Focus();
            quit = false;
            space = new Space();
            //space.collisionMode = true;
            start();
            
            centerX = Width / 2;
            centerY = Height / 2;

            scale = 1;

            drawSpeedInfo = true;
            //randomColorMode = false;

            drawSpeedVector = false;
            mouseX = -1;
            mouseY = -1;
            draggedX = -1;
            draggedY = -1;
            drawSpeed = null;
            speed = null;

            //Put default values to the inbuild body
            mass = DEFAULT_MASS;
            radius = DEFAULT_RADIUS;
            color = DEFAULT_COLOR;
            star = false;

            initTimer();
        }

        void SimPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //2nd point of the speed vector of the inbuild body
                draggedX = e.X;
                draggedY = e.Y;

                drawSpeed = new Vector2D(draggedX, draggedY);

                //Real speed of the inbuild body
                speed = new Vector2D(mouseX - e.X, mouseY - e.Y);
                speed = speed*mouseScale;
            }
        }

        void SimPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                addBody(new Body(mass, radius, color, star, 0, 0, 0, 0));
                drawSpeedVector = false;
            }
        }

        void SimPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //1st point of the speed vector of the inbuild body
                mouseX = e.X;
                mouseY = e.Y;
                draggedX = mouseX;
                draggedY = mouseY;

                drawSpeed = new Vector2D(mouseX, mouseY);
                speed = new Vector2D(0, 0);

                drawSpeedVector = true;

                //if (randomColorMode)
                //    color = ColorSelectorPanel.getRandomColor();
            }
            Focus();
        }

        void SimPanel_MouseClick(object sender, MouseEventArgs e)
        {
            Focus();
        }

        void SimPanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Keys key = e.KeyCode;
            if (key == Keys.Left ||
                key == Keys.Right ||
                key == Keys.Up ||
                key == Keys.Down)
                e.IsInputKey = true;
        }

        void SimPanel_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left: centerX += 10; break;
                case Keys.Right: centerX -= 10; break;
                case Keys.Up: centerY += 10; break;
                case Keys.Down: centerY -= 10; break;
            }
        }

        public static void run()
        {
            while (!SimPanel.quit)
            {
                space.tick();
                Thread.Sleep(10); //TODO : constant physic updates / second
            }
        }

        public static void start()
        {
            if (thread == null)
            {
                thread = new Thread(new ThreadStart(run));
                thread.Start();
            }
        }

        public void stop()
        {
            SimPanel.quit = true;
        }

        public void initTimer()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = (int)frameTime;
            timer.Start();
        }

        private void timerTick(object sender, EventArgs e)
        {
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            //base.OnPaint(pe);
            
            //Redessine tous les corps
            Graphics g = pe.Graphics;
            
            List<Body> bodies = space.getBodies();
            for(int i = 0; i < bodies.Count; i++)
            {
                Body body = bodies[i];

                //Dessine corps
                drawBody(g, body);

                //Dessine bulle info de la vitesse
                if (drawSpeedInfo)
                {
                    Point2D absPosition = relToAbsCoors(body.position.x, body.position.y);
                    int pixelRadius = radiusToAbs(body.radius, body.star);
                    int offset = pixelRadius + 2;
                    drawInfoBox(g, String.Format("{0}\n{1:N}", body.name, body.speed.length) + " m/s", absPosition.x, absPosition.y, offset);
                }
            }

            drawHUD(g);
            drawBodyToCreate(g);
        }

        private void drawBody(Graphics g, Body body)
        {
            Point2D absPosition = relToAbsCoors(body.position.x, body.position.y);
            int absRadius = radiusToAbs(body.radius, body.star);

            drawBody(g, body, absPosition.x, absPosition.y, absRadius);
        }

        private void drawBody(Graphics g, Body body, int absX, int absY, int absRadius)
        {
            /*
             * The coordinates of the objects drawn to the screen are calculated from the up-left corner
             * the coordinates of the bodies in the simulation are calculated from their center
             * we do a correction on the offset and we transform it in integer (pixel)
             */
            int x = (int)(absX - absRadius); //(px)
            int y = (int)(absY - absRadius); //(px)

            //Color
            Brush brush = new SolidBrush(body.color);

            //Draw body
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillEllipse(brush, x, y, absRadius*2, absRadius*2);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

            brush.Dispose();
        }

        private void drawBodyToCreate(Graphics g)
        {
            if (drawSpeedVector)
            {
                //Draw speed vector
                Pen pen = new Pen(Color.Red);
                g.DrawLine(pen, (int)(drawSpeed.x), (int)(drawSpeed.y), mouseX, mouseY);

                //Dessine le corps en construction
                int absRadius = radiusToAbs(radius, star);
                drawBody(g, new Body(absRadius, color, star, mouseX, mouseY), mouseX, mouseY, absRadius);
                drawInfoBox(g, String.Format("{0:N}", speed.length) + " m/s", draggedX, draggedY, 12);
            }
        }

        private void drawInfoBox(Graphics g, String text, int x, int y, int offset)
        {
            Font font = new Font("Tahoma", 10);

            int stringWidth = (int)(g.MeasureString(text, font).Width);
            int stringHeight = font.Height;

            //Draw the box
            int rectOffset = 2;
            int rectWidth = stringWidth + rectOffset*2;
            int rectHeight = stringHeight + rectOffset;
            int rectX = x + offset;
            int rectY = y + offset;
            Rectangle rect = new Rectangle(rectX, rectY, rectWidth, rectHeight*2);

            Brush brush = new SolidBrush(Color.FromArgb(128, Color.White));
            g.FillRectangle(brush, rect);

            //Draw the text
            TextFormatFlags flags = TextFormatFlags.HorizontalCenter |
                                    TextFormatFlags.VerticalCenter;
            TextRenderer.DrawText(g, text, font, rect, Color.Black, flags);

            brush.Dispose();
            font.Dispose();
        }

        private void drawHUD(Graphics g)
        {
            String infoText = "Planets: " + space.nbPlanets + "   " +
                              "Stars: " + space.nbStars + "   " +
                              "Date: " + space.getDate();

            Font font = new Font("Tahoma", 10);
            Brush brush = new SolidBrush(Color.White);
            g.DrawString(infoText, font, brush, 0, 0);

            font.Dispose();
            brush.Dispose();
        }

        public Point2D relToAbsCoors(double x, double y)
        {
            /* 
             * The calculus are done in their original units
             * the position of the bodies has to be modified to be seen in the window (positionScale)
             * the position of the bodies is relative to the center of the window, we have to transform it in absolute position
             */
            return new Point2D((int)(x * positionScale + centerX),
                               (int)(y * positionScale + centerY));
        }

        public int radiusToAbs(double radius, bool star)
        {
            return (int)(star ? radius * starRadiusScale : radius * bodyRadiusScale);
        }

        public void addBody(Body body)
        {
    		//Transform the absolute mouse position in position relative to the center
		    //Find where this position is in the simulation model (Scaling)
		    body.position.x = (mouseX - centerX)*1/positionScale;
            body.position.y = (mouseY - centerY)*1/positionScale;
		    body.speed = speed;
		    space.addBody(body);
	    }

        public void recenter()
        {
            centerX = Width / 2;
            centerY = Height / 2;
        }

        protected override void OnResize(EventArgs e)
        {
            recenter();
        }
    }
}
