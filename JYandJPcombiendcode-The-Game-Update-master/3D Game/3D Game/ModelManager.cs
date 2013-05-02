using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace _3D_Game
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ModelManager : DrawableGameComponent 
    {
        #region Manuel
        

       

        #endregion


        // List of models
        List<BasicModel> models = new List<BasicModel>();

        // Queue of Shooting Enemies
        List<ShootingEnemy> enemiesReadyToShoot = new List<ShootingEnemy>();

        // Spawn variables
        Vector3 maxSpawnLocation = new Vector3(100, 100, -3000);
        int nextSpawnTime = 0;
        int timeSinceLastSpawn = 0;
        float maxRollAngle = MathHelper.Pi / 40;
        public  double  playerHealth = 10;

        // Enemy count
        int enemiesThisLevel = 0;

        // Misses variables
        int missedThisLevel = 0;

        // Current level
       public int currentLevel = 0;

        // List of LevelInfo objects
        List<LevelInfo> levelInfoList = new List<LevelInfo>();

        // Shot stuff
        List<BasicModel> shots = new List<BasicModel>();
        float shotMinZ = -3000;

        //Special shots
        List<BasicModel> specialShots = new List<BasicModel>(); 

        //Enemy shots
        List<BasicModel> enemyShots = new List<BasicModel>();

        //Alliance 
        List<BasicModel> allianceList = new List<BasicModel>(); 

        //Speacial weapon and life up boxes
        List<BasicModel> lifeAndWeaponList = new List<BasicModel>();

        //List for mother ships around the space on both sides
        List<BasicModel> motherShips = new List<BasicModel>();

        //Variables for explosion
        List<ParticleExplosion> explosions = new List<ParticleExplosion>();
        ParticleExplosionSettings particleExplosionSettings = new ParticleExplosionSettings();
        ParticleSettings particleSettings = new ParticleSettings();
        Texture2D explosionTexture;
        Texture2D explosionColorsTexture;
        Effect explosionEffect;

        //variables for stars
        ParticleStarSheet stars;
        Effect starEffect;
        Texture2D starTexture;

        //points worth for each ship
        const int pointsPerKill = 20;

        //For power up track
        public int consecutiveKills = 0;
        int rapidFireKillRequirement = 3;

    
        // The mother ship checking in game
            int mothership = 0;

      
        public
            int alliance = 0;
            int lifeAndWeapon = 0;
            int colliedTrackForLifeAndWeapon = 0;
            int healthToCheckTheGeneratingFunction = 0;
      

        public ModelManager(Game game)
            : base(game)
        {
            // Initialize game levels
            levelInfoList.Add(new LevelInfo(1000, 3000, 20, 2, 6, 10));
            levelInfoList.Add(new LevelInfo(900, 2800, 22, 2, 6, 9));
            levelInfoList.Add(new LevelInfo(800, 2600, 24, 2, 6, 8));
            levelInfoList.Add(new LevelInfo(700, 2400, 26, 3, 7, 7));
            levelInfoList.Add(new LevelInfo(600, 2200, 28, 3, 7, 6));
            levelInfoList.Add(new LevelInfo(500, 2000, 30, 3, 7, 5));
            levelInfoList.Add(new LevelInfo(400, 1800, 32, 4, 7, 4));
            levelInfoList.Add(new LevelInfo(300, 1600, 34, 4, 8, 3));
            levelInfoList.Add(new LevelInfo(200, 1400, 36, 5, 8, 2));
            levelInfoList.Add(new LevelInfo(100, 1200, 38, 5, 9, 1));
            levelInfoList.Add(new LevelInfo(50, 1000, 40, 6, 9, 0));
            levelInfoList.Add(new LevelInfo(50, 800, 42, 6, 9, 0));
            levelInfoList.Add(new LevelInfo(50, 600, 44, 8, 10, 0));
            levelInfoList.Add(new LevelInfo(25, 400, 46, 8, 10, 0));
            levelInfoList.Add(new LevelInfo(0, 200, 48, 18, 20, 0));
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // Set initial spawn time
            SetNextSpawnTime();
            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Load explosion texture and effects
            explosionTexture = Game.Content.Load<Texture2D>(@"Textures\Particle");
            explosionColorsTexture = Game.Content.Load<Texture2D>(@"Textures\ParticleColors");
            explosionEffect = Game.Content.Load<Effect>(@"effects\particle");

            //set effect parameters that don't change per particle
            explosionEffect.CurrentTechnique = explosionEffect.Techniques["Technique1"];
            explosionEffect.Parameters["theTexture"].SetValue(explosionTexture);

            //Load the stars
            starTexture = Game.Content.Load<Texture2D>(@"textures\stars");
            starEffect = explosionEffect.Clone();
            starEffect.CurrentTechnique = starEffect.Techniques["Technique1"];
            starEffect.Parameters["theTexture"].SetValue(explosionTexture);

            //Initialize particle star sheet
            stars = new ParticleStarSheet(GraphicsDevice,
                new Vector3(2000,2000,-1900),
                1500, starTexture,
                particleSettings,
                starEffect);

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Check to see if it's time to spawn
            CheckToSpawnEnemy(gameTime);

            // Update models
            UpdateModels();

            //Update Alliance
            UpdateSpawnAlliance();

            //Updat MotherShips and Planets
            UpdateSpawnMotherShipsAndPlants();

            //Update life and weapon box
            UpdateSpawnMissileAndLife();

            // Update shots
            UpdateShots();

            //Update Special Shot
            UpdateSpecialShots();

            //Update Enemy Shots
            UpdateEnemyShots();

            
            //Update the explosion
            UpdateExplosion(gameTime);

            base.Update(gameTime);
        }

        protected void UpdateModels()
        {
            // Loop through all models and call Update
            
            for (int i = 0; i < models.Count; ++i)
            {
                // Update each model
                models[i].Update();

                
                       if (((models[i].GetWorld().Translation.X > 50 ) &&
                        (models[i].GetWorld().Translation.X < 500)) &&
                        (models[i].GetWorld().Translation.Z >
                            ((Game1)Game).camera.cameraPosition.Z - 10))
                                        
                        {   
                 
                          if ((missedThisLevel >
                                levelInfoList[currentLevel].missesAllowed) || (playerHealth < 1)) // Die if health == 0
                            {
                                ((Game1)Game).ChangeGameState(
                                    Game1.GameState.END, currentLevel);
                            }
                 
                          //  ++missedThisLevel;

                           models.RemoveAt(i);
                            --i;
                            playerHealth -= 1;
                            ((Game1)Game).PlayCue("hit1");
                            ((Game1)Game).blowup = true;
                            ((Game1)Game).UpdateExplosions(((Game1)Game).getGameTime());
                        }

                        else if (((models[i].GetWorld().Translation.Y > 50) &&
                         (models[i].GetWorld().Translation.Y < 300)) &&
                        (models[i].GetWorld().Translation.Z >
                            ((Game1)Game).camera.cameraPosition.Z -10))
                        {
                             
                        if ((missedThisLevel >
                                levelInfoList[currentLevel].missesAllowed) || (playerHealth < 1)) // Die if health == 0
                            {
                                ((Game1)Game).ChangeGameState(
                                    Game1.GameState.END, currentLevel);
                            }
                  
                 
                         //   ++missedThisLevel;
                            models.RemoveAt(i);
                            --i;
                            playerHealth -= 1;
                            ((Game1)Game).PlayCue("hit1");
                            ((Game1)Game).blowup = true;
                            ((Game1)Game).UpdateExplosions(((Game1)Game).getGameTime());
                        }


                        else if ((i != -1) && (models[i].GetWorld().Translation.Z >
                            ((Game1)Game).camera.cameraPosition.Z + 200))
                        {
                           
                            enemiesThisLevel--;
                            SpawnEnemy();

                            ++missedThisLevel;
                          
                            if (missedThisLevel >
                                levelInfoList[currentLevel].missesAllowed)
                            {
                                ((Game1)Game).ChangeGameState(
                                    Game1.GameState.END, currentLevel);
                            }
                            //Reset the kill count
                            consecutiveKills = 0;
                            models.RemoveAt(i);
                          --i;
                          ((Game1)Game).blowup = true;
                          ((Game1)Game).UpdateExplosions(((Game1)Game).getGameTime());
                        }

                            else if (models[i] is ShootingEnemy)
                         {
                             enemiesReadyToShoot.Add((ShootingEnemy)models[i]);
                         }

                        else
                        {
                            // If shot is still in play, check for collisions
                            for (int j = 0; j < shots.Count; ++j)
                            {

                                if (models[i].CollidesWith(shots[j].model,
                                    shots[j].GetWorld()))
                                {
                                    shots.RemoveAt(j);
                                    models.RemoveAt(i);
                                    --i;

                                    ((Game1)Game).PlayCue("Explosions");
                                    break;
                                }
                            }
                        }
                          
                
        
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Loop through and draw each model
            foreach (BasicModel bm in models)
            {
                bm.Draw(((Game1)Game).camera);
            }

            //Loop through the Alliance
            foreach (BasicModel bm in allianceList)
            {
                bm.Draw(((Game1)Game).camera);
            }

            //Loop through the life and missile box
            foreach (BasicModel bm in lifeAndWeaponList)
            {
                bm.Draw(((Game1)Game).camera);
            }

            //For motherships
            foreach (BasicModel bm in motherShips) 
            {
                bm.Draw(((Game1)Game).camera);
            }

            // Loop through and draw each shot
            foreach (BasicModel bm in shots)
            {
                bm.Draw(((Game1)Game).camera);
            }

            // Loop through and draw each special shot
            foreach (BasicModel bm in specialShots) 
            {
                bm.Draw(((Game1)Game).camera);
            }

            // Loop through and draw each enemy shot
            foreach (BasicModel bm in enemyShots)
            {
                bm.Draw(((Game1)Game).camera);
            }


            foreach (ParticleExplosion pe in explosions)
            {
                pe.Drew(((Game1)Game).camera);
            }

            stars.Drew(((Game1)Game).camera);
            base.Draw(gameTime);
        }


        private void SetNextSpawnTime()
        {
            // Reset the variables to indicate the next enemy spawn time
            nextSpawnTime = ((Game1)Game).rnd.Next(
                levelInfoList[currentLevel].minSpawnTime,
                levelInfoList[currentLevel].maxSpawnTime);
            timeSinceLastSpawn = 0;
        }


        protected void CheckToSpawnEnemy(GameTime gameTime)
        {
            // Time to spawn a new enemy?
            if (enemiesThisLevel <
                levelInfoList[currentLevel].numberEnemies)
            {
                timeSinceLastSpawn += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastSpawn > nextSpawnTime)
                {
                    SpawnEnemy();
                    SpawnAlliance();
                    SpawnMissileAndLife();
                    SpawnMotherShipsAndPlants();
                }
            }

            else
            {
                //Reset for every room
                if (explosions.Count == 0 && models.Count == 0)
                {
                    // All explosion and ships are removed and the level is over
                    ++currentLevel;
                    enemiesThisLevel = 0;
                    missedThisLevel = 0;
                    alliance = 0;
                    lifeAndWeapon = 0;
                    healthToCheckTheGeneratingFunction = 0;
                    
                    for (int i = 0; i < lifeAndWeaponList.Count ; ++i) 
                    {
                        // Update each Alliance
                        lifeAndWeaponList.RemoveAt(i);
                        
                    }
                    

                    ((Game1)Game).ChangeGameState(
                        Game1.GameState.LEVEL_CHANGE,
                        currentLevel);
                }
            }
        }

        private void SpawnEnemy()
        {
            // Generate random position with random X and random Y
            // between -maxX and maxX and -maxY and maxY. Z is always
            // the same for all ships.
            Vector3 position = new Vector3(((Game1)Game).rnd.Next(
                -(int)maxSpawnLocation.X, (int)maxSpawnLocation.X),
                ((Game1)Game).rnd.Next(
                -(int)maxSpawnLocation.Y, (int)maxSpawnLocation.Y),
                maxSpawnLocation.Z);

 
            // Direction will always be (0, 0, Z), where
            // Z is a random value between minSpeed and maxSpeed
            Vector3 direction = new Vector3(0, 0,
                ((Game1)Game).rnd.Next(
                levelInfoList[currentLevel].minSpeed,
                levelInfoList[currentLevel].maxSpeed));

 
            // Get a random roll rotation between -maxRollAngle and maxRollAngle
            float rollRotation = (float)((Game1)Game).rnd.NextDouble() *
                    maxRollAngle - (maxRollAngle / 2);
            
            //25% chance to spawn shooting enemy
            if (((Game1)Game).rnd.Next(8) <= 1)
            {
                // Add shooting enemy
                models.Add(new ShootingEnemy(
                Game.Content.Load<Model>(@"models\spaceship"),
                position, direction, 0, 0, 0));
            }
            else
            {
                // Add model to the list
                models.Add(new SpinningEnemy(
                    Game.Content.Load<Model>(@"models\spaceship"),
                    position, direction, 0, 0, rollRotation));
            }
                     
            
            // Increment # of enemies this level and set next spawn time
            ++enemiesThisLevel;
            SetNextSpawnTime();
        }

        // For spawn life and missile boxes

        private void SpawnMotherShipsAndPlants()
        {
            // This for planets and mother ships 
            if (mothership == 0)
            {
                motherShips.Add(new SpinningEnemy(
                   Game.Content.Load<Model>(@"models\missile"),
                   new Vector3(-12, 10, -15), new Vector3(0, 0, 0), 0, 0, -0.1f));

                motherShips.Add(new SpinningEnemy(
                   Game.Content.Load<Model>(@"models\missile"),
                   new Vector3(12, 10, -15), new Vector3(0, 0, 0), 0, 0, 0.1f));

                motherShips.Add(new SpinningEnemy(
                   Game.Content.Load<Model>(@"models\Mothership"),
                   new Vector3(-120, 100, -100), new Vector3(0.2f, 0, 0), 0, 0, 0));

                ++mothership;
            }
        }

        public void UpdateSpawnMotherShipsAndPlants()
        {
           
           
            for (int i = 0; i < motherShips.Count; ++i)
            {
                motherShips[i].Update();
                if ((motherShips[i].GetWorld().Translation.X < -700) || (motherShips[i].GetWorld().Translation.X > 800))
                {
                    motherShips.RemoveAt(i);
                    --i;
                }
               
            }
        }


        private void SpawnMissileAndLife()
        {
            //Add speacial weapon and life box 

            Vector3 lifeAndWeaponPosition = new Vector3(((Game1)Game).rnd.Next(
                -(int)maxSpawnLocation.X, (int)maxSpawnLocation.X),
                ((Game1)Game).rnd.Next(
                -(int)maxSpawnLocation.Y, (int)maxSpawnLocation.Y),
                -200);

            Vector3 lifeAndWeaponDirection = new Vector3(0, 0, 0); // -20 its the speed of the spaceship

            
            if (lifeAndWeapon < 2)
            {
                if (healthToCheckTheGeneratingFunction == 0)
                {
                    lifeAndWeaponList.Add(new SpinningEnemy(
                   Game.Content.Load<Model>(@"models\ship1"),
                   lifeAndWeaponPosition, lifeAndWeaponDirection, -0.1f, 0, 0)); //For Yaw speed
                    ++lifeAndWeapon;
                    healthToCheckTheGeneratingFunction = 1;
                }
                else
                {


                    lifeAndWeaponList.Add(new SpinningEnemy(
                       Game.Content.Load<Model>(@"models\EnemySpaceship"),
                       lifeAndWeaponPosition, lifeAndWeaponDirection, -0.1f, 0, 0)); //For Yaw speed
                    ++lifeAndWeapon;
                }
            }

        }

        public void UpdateSpawnMissileAndLife()
        {
            for (int i = 0; i < lifeAndWeaponList.Count; ++i)
            {
                // Update each Alliance
                lifeAndWeaponList[i].Update();
            }

            
        }

        private void SpawnAlliance() 
        {
            // for spawn The alliance ship
            // Add alliance objects that will fly from camera side to the depth 

            Vector3 alliancePosition = new Vector3(((Game1)Game).rnd.Next(
                -(int)maxSpawnLocation.X, (int)maxSpawnLocation.X) +50,
                ((Game1)Game).rnd.Next(
                -(int)maxSpawnLocation.Y, (int)maxSpawnLocation.Y),
                -10);
            
            Vector3 allianceDirection = new Vector3(0, 0, -2); // The speed of the spaceship 

            if (alliance < 5)
            {
                allianceList.Add(new SpinningEnemy(
                Game.Content.Load<Model>(@"models\ship1"),
                alliancePosition, allianceDirection, 0, 0, 0)); //The rolling speed
               
                ((Game1)Game).PlayCue("UnitReady");
                ++alliance;

            }
          
        }

        public void UpdateSpawnAlliance() 
        {
            for (int i = 0; i < allianceList.Count; ++i)
            {
                // Update each Alliance
                allianceList[i].Update();

                if (allianceList[i].GetWorld().Translation.Z < shotMinZ)
                {
                    allianceList.RemoveAt(i);
                    --i;
                }
                else
                {
                    // If shot is still in play, check for collisions
                    for (int j = 0; j < models.Count; ++j)
                    {

                        if (allianceList[i].CollidesWith(models[j].model,
                            models[j].GetWorld()))
                        {
                            //clolision add explosion 
                            explosions.Add(new ParticleExplosion(GraphicsDevice,
                                models[j].GetWorld().Translation,
                                ((Game1)Game).rnd.Next(
                                particleExplosionSettings.minLife,
                                particleExplosionSettings.maxLife),
                                ((Game1)Game).rnd.Next(
                                particleExplosionSettings.minRoundTime,
                                particleExplosionSettings.maxRoundTime),
                                ((Game1)Game).rnd.Next(
                                particleExplosionSettings.minParticlePerRound,
                                particleExplosionSettings.maxParticlePreRound),
                                ((Game1)Game).rnd.Next(
                                particleExplosionSettings.minParticles,
                                particleExplosionSettings.maxParticles),
                                explosionColorsTexture, particleSettings,
                                explosionEffect));


                            ((Game1)Game).AddPoints(pointsPerKill * (currentLevel + 1));     
                                // Collision! Remove the ship and the shot.
                            models.RemoveAt(j);
                            allianceList.RemoveAt(i);
                            --i;
                            ((Game1)Game).PlayCue("UnitLost");
                            ((Game1)Game).PlayCue("Explosions");
                           
                           break;
                        }
                    }
                }
                
            }
            
            
        }

        

        public void AddShot(Vector3 position, Vector3 direction)
        {
            shots.Add(new SpinningEnemy(
                Game.Content.Load<Model>(@"models\skpfile"),
                position, direction, 0, 0, 0));
        }

        public void AddSpecialShots(Vector3 position, Vector3 direction) 
        {
            if (models.Count > 0)
            {
                Vector3 closestEnemyShip = models[0].world.Translation;
                for (int j = 1; j < models.Count; ++j)
                {
                    if (closestEnemyShip.Z < models[j].world.Translation.Z)
                        closestEnemyShip = models[j].world.Translation;
                }
                specialShots.Add(new Missile(Game.Content.Load<Model>(@"models\AMRAAM"),
                    position, direction, 0, 0, 0, closestEnemyShip));

  
            }
        }

        protected void UpdateSpecialShots() 
        {
            for (int i = 0; i < specialShots.Count; ++i)
            {
                // Update each shot
                specialShots[i].Update();

                // If shot is out of bounds, remove it from game
                if (specialShots[i].GetWorld().Translation.Z < shotMinZ)
                {
                    specialShots.RemoveAt(i);
                    --i;
                }



                else if ((lifeAndWeaponList.Count > 0) && (i > -1)) 
                {
                    for (int k = 0; k < lifeAndWeaponList.Count; ++k)
                    {

                        if ((i != -1) && (specialShots[i].CollidesWith(lifeAndWeaponList[k].model,
                           lifeAndWeaponList[k].GetWorld())))
                        {
                            explosions.Add(new ParticleExplosion(GraphicsDevice,
                           lifeAndWeaponList[k].GetWorld().Translation,
                           ((Game1)Game).rnd.Next(
                           particleExplosionSettings.minLife,
                           particleExplosionSettings.maxLife),
                           ((Game1)Game).rnd.Next(
                           particleExplosionSettings.minRoundTime,
                           particleExplosionSettings.maxRoundTime),
                           ((Game1)Game).rnd.Next(
                           particleExplosionSettings.minParticlePerRound,
                           particleExplosionSettings.maxParticlePreRound),
                           ((Game1)Game).rnd.Next(
                           particleExplosionSettings.minParticles,
                           particleExplosionSettings.maxParticles),
                           explosionColorsTexture, particleSettings,
                           explosionEffect));

                            lifeAndWeaponList.RemoveAt(k);
                            specialShots.RemoveAt(i);
                            --k;
                            ++colliedTrackForLifeAndWeapon;
                            ((Game1)Game).PlayCue("Explosions");
                            break;

                        }
                        else 
                        {
                            for (int j = 0; j < models.Count; ++j)
                            {

                           
                                //Check if shot colid with powerup or lifeup
                                // check if shot colid with enemy
                                if ((i != -1) && specialShots[i].CollidesWith(models[j].model,
                                    models[j].GetWorld()))
                                {
                                    //clolision add explosion 
                                    explosions.Add(new ParticleExplosion(GraphicsDevice,
                                        models[j].GetWorld().Translation,
                                        ((Game1)Game).rnd.Next(
                                        particleExplosionSettings.minLife,
                                        particleExplosionSettings.maxLife),
                                        ((Game1)Game).rnd.Next(
                                        particleExplosionSettings.minRoundTime,
                                        particleExplosionSettings.maxRoundTime),
                                        ((Game1)Game).rnd.Next(
                                        particleExplosionSettings.minParticlePerRound,
                                        particleExplosionSettings.maxParticlePreRound),
                                        ((Game1)Game).rnd.Next(
                                        particleExplosionSettings.minParticles,
                                        particleExplosionSettings.maxParticles),
                                        explosionColorsTexture, particleSettings,
                                        explosionEffect));


                                    ((Game1)Game).AddPoints(pointsPerKill * (currentLevel + 1));
                                    // Collision! Remove the ship and the shot.
                                    models.RemoveAt(j);
                                    specialShots.RemoveAt(i);
                                    --i;
                                    ((Game1)Game).PlayCue("Explosions");
                                    //Update tje consecutive kill count
                                    ++consecutiveKills;

                                    break;
                                }



                            }

                        } 
                    }
                }
                else
                {
                    for (int j = 0; j < models.Count; ++j)
                    {
                        //Check if shot colid with powerup or lifeup
                        //Check if shot colid with enemy
                        if (specialShots[i].CollidesWith(models[j].model,
                            models[j].GetWorld()))
                        {
                            //clolision add explosion 
                            explosions.Add(new ParticleExplosion(GraphicsDevice,
                                models[j].GetWorld().Translation,
                                ((Game1)Game).rnd.Next(
                                particleExplosionSettings.minLife,
                                particleExplosionSettings.maxLife),
                                ((Game1)Game).rnd.Next(
                                particleExplosionSettings.minRoundTime,
                                particleExplosionSettings.maxRoundTime),
                                ((Game1)Game).rnd.Next(
                                particleExplosionSettings.minParticlePerRound,
                                particleExplosionSettings.maxParticlePreRound),
                                ((Game1)Game).rnd.Next(
                                particleExplosionSettings.minParticles,
                                particleExplosionSettings.maxParticles),
                                explosionColorsTexture, particleSettings,
                                explosionEffect));


                            ((Game1)Game).AddPoints(pointsPerKill * (currentLevel + 1));
                            // Collision! Remove the ship and the shot.
                            models.RemoveAt(j);
                            specialShots.RemoveAt(i);
                            --i;
                            ((Game1)Game).PlayCue("Explosions");
                            //Update tje consecutive kill count
                            ++consecutiveKills;

                            break;
                        }



                    }

                }
            }

        }


        protected void UpdateEnemyShots() // creates and updates enemy bullets
        {
            //Make enemies create shots
            for (int i = 0; i < enemiesReadyToShoot.Count; ++i )
            {
                if (enemiesReadyToShoot[i].getShoot())
                {
                    enemyShots.Add(new SpinningEnemy(
                        Game.Content.Load<Model>(@"models\spaceship"),
                        enemiesReadyToShoot[i].getPosition(), enemiesReadyToShoot[i].getDirection() * 2, 0, 0, 0));
                    enemiesReadyToShoot[i].setShoot(false);
                    enemiesReadyToShoot.RemoveAt(i);
                    --i;
                }
            }

            //Update enemy shots
            for (int i = 0; i < enemyShots.Count; ++i)
            {
                // Update each shot
                enemyShots[i].Update();


                if (((enemyShots[i].GetWorld().Translation.X > 50) &&
                (enemyShots[i].GetWorld().Translation.X < 300)) &&
                (enemyShots[i].GetWorld().Translation.Z >
                    ((Game1)Game).camera.cameraPosition.Z - 10))
                                        
                {
                    enemyShots.RemoveAt(i);
                    --i;
                    playerHealth -= 0.2;
                    ((Game1)Game).PlayCue("GlassScrunch4");
                }

                else if (((enemyShots[i].GetWorld().Translation.Y > 50) &&
                 (enemyShots[i].GetWorld().Translation.Y < 300 )) && 
                     (enemyShots[i].GetWorld().Translation.Z >
                    ((Game1)Game).camera.cameraPosition.Z - 10))
                {
                    enemyShots.RemoveAt(i);
                    --i;
                    playerHealth -= 0.2;
                    ((Game1)Game).PlayCue("GlassScrunch4");
                }
                

                else if (enemyShots[i].GetWorld().Translation.Z >
                    ((Game1)Game).camera.cameraPosition.Z + 200)
                {
                    enemyShots.RemoveAt(i);
                    --i;
                  
                }
                else
                {
                    // If shot is still in play, check for collisions
                    for (int j = 0; j < shots.Count; ++j)
                    {

                        if (enemyShots[i].CollidesWith(shots[j].model,
                            shots[j].GetWorld()))
                        {
                            shots.RemoveAt(j);
                            enemyShots.RemoveAt(i);
                            --i;

                            ((Game1)Game).PlayCue("Explosions");
                            break;
                        }
                    }
                }
            }
        }

        protected void UpdateShots()
        {
            // Loop through shots

            for (int i = 0; i < shots.Count; ++i)
            {
                // Update each shot
                shots[i].Update();

                // If shot is out of bounds, remove it from game
                if (shots[i].GetWorld().Translation.Z < shotMinZ)
                {
                    shots.RemoveAt(i);
                    --i;
                }



                else if ((lifeAndWeaponList.Count > 0) && (i != -1)) 
                {
                    for (int k = 0; k < lifeAndWeaponList.Count; ++k)
                    {
                        if ((i != -1) && (shots[i].CollidesWith(lifeAndWeaponList[k].model,
                           lifeAndWeaponList[k].GetWorld())))
                        {
                            
                            explosions.Add(new ParticleExplosion(GraphicsDevice,
                           lifeAndWeaponList[k].GetWorld().Translation,
                           ((Game1)Game).rnd.Next(
                           particleExplosionSettings.minLife,
                           particleExplosionSettings.maxLife),
                           ((Game1)Game).rnd.Next(
                           particleExplosionSettings.minRoundTime,
                           particleExplosionSettings.maxRoundTime),
                           ((Game1)Game).rnd.Next(
                           particleExplosionSettings.minParticlePerRound,
                           particleExplosionSettings.maxParticlePreRound),
                           ((Game1)Game).rnd.Next(
                           particleExplosionSettings.minParticles,
                           particleExplosionSettings.maxParticles),
                           explosionColorsTexture, particleSettings,
                           explosionEffect));
                            
                                                       
                            if (healthToCheckTheGeneratingFunction == 1)
                            {
                                playerHealth += 5;
                                healthToCheckTheGeneratingFunction = 2;
                                ((Game1)Game).PlayCue("LifeAndMissile");
                            }
                            else
                            {
                                ((Game1)Game).specialList += 3;
                                ((Game1)Game).PlayCue("LifeAndMissile");
                            }

                            lifeAndWeaponList.RemoveAt(k);
                            shots.RemoveAt(i);
                            --k;
                            ++colliedTrackForLifeAndWeapon;
                          
                            break;

                        }
                        else
                        {
                            for (int j = 0; j < models.Count; ++j)
                            {
                                //Check if shot colid with powerup or lifeup

                                // check if shot colid with enemy
                                if ((i != -1) && shots[i].CollidesWith(models[j].model,
                                    models[j].GetWorld()))
                                {
                                    //clolision add explosion 
                                    explosions.Add(new ParticleExplosion(GraphicsDevice,
                                        models[j].GetWorld().Translation,
                                        ((Game1)Game).rnd.Next(
                                        particleExplosionSettings.minLife,
                                        particleExplosionSettings.maxLife),
                                        ((Game1)Game).rnd.Next(
                                        particleExplosionSettings.minRoundTime,
                                        particleExplosionSettings.maxRoundTime),
                                        ((Game1)Game).rnd.Next(
                                        particleExplosionSettings.minParticlePerRound,
                                        particleExplosionSettings.maxParticlePreRound),
                                        ((Game1)Game).rnd.Next(
                                        particleExplosionSettings.minParticles,
                                        particleExplosionSettings.maxParticles),
                                        explosionColorsTexture, particleSettings,
                                        explosionEffect));


                                    ((Game1)Game).AddPoints(pointsPerKill * (currentLevel + 1));
                                    // Collision! Remove the ship and the shot.
                                    models.RemoveAt(j);
                                    shots.RemoveAt(i);
                                    --i;
                                    ((Game1)Game).PlayCue("Explosions");
                                    //Update tje consecutive kill count
                                    ++consecutiveKills;

                                    if (consecutiveKills == rapidFireKillRequirement)
                                    {
                                        ((Game1)Game).StartPowerUp(Game1.PowerUps.RAPID_FIRE);
                                    }


                                    break;
                                }



                            }

                        } 
                    }
                }
                else
                {
                    for (int j = 0; j < models.Count; ++j)
                    {
                        //Check if shot colid with powerup or lifeup
                        
                        // check if shot colid with enemy
                        if (shots[i].CollidesWith(models[j].model,
                            models[j].GetWorld()))
                        {
                            //clolision add explosion 
                            explosions.Add(new ParticleExplosion(GraphicsDevice,
                                models[j].GetWorld().Translation,
                                ((Game1)Game).rnd.Next(
                                particleExplosionSettings.minLife,
                                particleExplosionSettings.maxLife),
                                ((Game1)Game).rnd.Next(
                                particleExplosionSettings.minRoundTime,
                                particleExplosionSettings.maxRoundTime),
                                ((Game1)Game).rnd.Next(
                                particleExplosionSettings.minParticlePerRound,
                                particleExplosionSettings.maxParticlePreRound),
                                ((Game1)Game).rnd.Next(
                                particleExplosionSettings.minParticles,
                                particleExplosionSettings.maxParticles),
                                explosionColorsTexture, particleSettings,
                                explosionEffect));


                            ((Game1)Game).AddPoints(pointsPerKill * (currentLevel + 1));
                            // Collision! Remove the ship and the shot.
                            models.RemoveAt(j);
                            shots.RemoveAt(i);
                            --i;
                            ((Game1)Game).PlayCue("Explosions");
                            //Update tje consecutive kill count
                            ++consecutiveKills;

                            if (consecutiveKills == rapidFireKillRequirement)
                            {
                                ((Game1)Game).StartPowerUp(Game1.PowerUps.RAPID_FIRE);
                            }


                            break;
                        }



                    }

                }
            }
        }


        protected void UpdateExplosion(GameTime gameTime)
        {
            for (int i = 0; i < explosions.Count; ++i)
            {
                explosions[i].Update(gameTime);

                //if explosion is finished just remove it
                if (explosions[i].IsDead)
                {
                    explosions.RemoveAt(i);
                    --i;
                }
            }

           
            
        }

        public int missesLeft
        {
            get
            {
                return
                    levelInfoList[currentLevel].missesAllowed
                    - missedThisLevel;
            }
        }
    }
}
