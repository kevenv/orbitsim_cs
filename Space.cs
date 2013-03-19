using System;
using System.Collections.Generic;
using System.Drawing;

namespace OrbitSim
{
    /// <summary>
    /// The space in wich the bodies interacts.
    /// </summary>
    class Space
    {
        private List<Body> bodies; //Bodies list of the model
        public int nbPlanets { get; private set; }
        public int nbStars { get; private set; }

        public bool realisticMode { get; set; } //True if the simulator is in realistic mode :
                                                //The acceleration of the star will be calculated
        public bool collisionMode { get; set; } //True if the simulator has to calculate collisions

        public long dt { get; set; } //Time step (s)

        public long timer { get; private set; } //Simulated time counter (s)
        private System.DateTime date;

        /// <summary>
        /// Create the simulation's space.
        /// </summary>
        public Space()
        {
            bodies = new List<Body>();
            nbPlanets = 0;
            nbStars = 0;
            
            realisticMode = false;
            collisionMode = false;

            dt = 60*60; //TODO : 1s réelle = 36 000s simulées = 10h
            timer = 0;
            date = new DateTime(1, 1, 1, 0, 0, 0);

            loadSolarSystem();
        }

        /// <summary>
        /// Put the solar system in the simulation's space.
        /// </summary>
        private void loadSolarSystem()
        {
            //Positions are relatives to the centre of the simulation
            //ex: x = 0, y = 5 then position is x = centerX + 0, y = centerY + 0
            //mass, radius, color, star, x(local), y(local), vx, vy	
            bodies.Add(new Body("Sun", 1.9891e30, 695500000, Color.Yellow, true, 0, 0, 0, 0));
            bodies.Add(new Body("Mercury", 3.33e3, 2440000, Color.Gray, false, 57900000000.0, 0, 0, 47900));
            bodies.Add(new Body("Venus", 4.869E24, 6050000, Color.Orange, false, 108000000000.0, 0, 0, 35000));
            bodies.Add(new Body("Earth", 5.9736E24, 6378100, Color.Blue, false, 1.5E11, 0, 0, 29800));
            bodies.Add(new Body("Moon", 7.3477E22, 1738100, Color.LightGray, false, 1.50384E11, 0, 0, 29800 + 1022));
            bodies.Add(new Body("Mars", 6.421E23, 3397200, Color.Red, false, 227940000000.0, 0, 0, 24100));
            bodies.Add(new Body("Jupiter", 1.9E+027, 71492000, Color.Orange, false, 778330000000.0, 0, 0, 13100));
            bodies.Add(new Body("Saturn", 5.688E+026, 60268000, Color.Yellow, false, 1429400000000.0, 0, 0, 9640));
            bodies.Add(new Body("Uranus", 8.686E+025, 25559000, Color.Blue, false, 2870990000000.0, 0, 0, 6810));
            bodies.Add(new Body("Neptune", 1.024E+026, 24746000, Color.Blue, false, 4504300000000.0, 0, 0, 5430));
            bodies.Add(new Body("Pluto", 1.305E+22, 1153000, Color.Gray, false, 7.311E+12, 0, 0, 4666));

            nbPlanets = 10;
            nbStars = 1;
        }

        /// <summary>
        /// Simulate 1 physic frame.
        /// </summary>
        public void tick()
        {
            //For every bodies
            Body A = null;
            Body B = null;

            for(int i  = 0; i < bodies.Count; i++)
            {
                A = bodies[i];
                if (!realisticMode && A.star)
                    continue;

                //Calculate the gravity effect that every body do
                for(int j = 0; j < bodies.Count; j++)
                {
                    B = bodies[j];
                    //Collisions
                    if (collisionMode && A.isColliding(B))
                        collide(A, B);

                    //Gravitational acceleration
                    A.calcAcceleration(B, dt);
                }

                //Move body
                A.move(dt);
            }

            //Update timer
            timer += dt;
        }

        /// <summary>
        /// Remove every bodies of the simulation.
        /// </summary>
        public void clear()
        {
            bodies.Clear();
            nbPlanets = 0;
            nbStars = 0;
        }

        /// <summary>
        /// Reinit the simulation.
        /// </summary>
        public void reset()
        {
            clear();
            timer = 0;
            date = new DateTime(1, 1, 1, 0, 0, 0);
            loadSolarSystem();
        }

        /// <summary>
        /// Do the collision of 2 bodies.
        /// </summary>
        /// <param name="A">Body A</param>
        /// <param name="B">Body B</param>
        private void collide(Body A, Body B)
        {
            if (A != B)
            {
                //A < B
                if (A.mass < B.mass)
                {
                    B.collide(A);
                    //explode(A, B, 1);
                    removeBody(A);
                }
                //A > B
                else if (A.mass > B.mass)
                {
                    A.collide(B);
                    //explode(A, B, 2);
                    removeBody(B);
                }
                //A == B
                else
                {
                    //explode(A, B, 0);
                    removeBody(A);
                    removeBody(B);
                }
            }
        }

        /// <summary>
        /// Add a body to the simulation.
        /// </summary>
        /// <param name="body">Body to add.</param>
        public void addBody(Body body)
        {
            bodies.Add(body);
            if (body.star)
                nbStars++;
            else
                nbPlanets++;
        }

        /// <summary>
        /// Remove a body from the simulation.
        /// </summary>
        /// <param name="body">Body to remove.</param>
        public void removeBody(Body body)
        {
            if (body.star)
                nbStars--;
            else
                nbPlanets--;
            bodies.Remove(body);
        }

        /// <summary>
        /// Return the bodies list of the simulation.
        /// </summary>
        /// <returns></returns>
        public List<Body> getBodies()
        {
            return bodies;
        }

        public void setCollisionScales(double bodyRadiusScale, double starRadiusScale, double positionScale)
        {
            Body.setCollisionScales(bodyRadiusScale, starRadiusScale, positionScale);
        }

        public String getDate()
        {
            date = new DateTime(1, 1, 1, 0, 0, 0).AddSeconds(timer);
            return date.ToString("dd-MM-yyyy hh:mm:ss");
        }

        public override string ToString()
        {
            String s = "";
            foreach (Body body in bodies)
                s = s + body + "\n";
            s = "--------" + s;
            return s;
        }
    }
}
