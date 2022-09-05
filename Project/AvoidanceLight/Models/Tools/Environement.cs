using AvoidanceLight.Models.Entity;
using System.Windows.Input;

namespace AvoidanceLight.Models.Tools
{
    public class Environement
    {
        private static Environement? _instance;
        public static Environement GetInstance(int XScreenSize,int YScreenSize,int PopulationSize)
        {
            if (Environement._instance == null)
            {
                Environement._instance = new Environement(XScreenSize,YScreenSize,PopulationSize);
            }
            return Environement._instance;
        }
        
        public List<BaseEntity> listEntities {get; private set;}
        public List<Ero> listEros {get; private set;}
        public List<Nemie> listNemies {get; private set;}
        public Screen MainScreen {get; private set;}
        private int populationSize;
        private long spawningTime = 100;
        private Random rdn = new Random();

        public Environement(int XScreenSize,int YScreenSize,int PopulationSize)
        {
            MainScreen = new Screen(XScreenSize,YScreenSize);
            populationSize = PopulationSize;
            listEros = new List<Ero>();
            listNemies = new List<Nemie>();
            listEntities = new List<BaseEntity>();
        }

        public void Setup()
        {
            for(int i = 0; i < populationSize;i++)
            {
                this.AddEro();
            }

            this.AddEnemy(rdn.Next(0,MainScreen.Xsize-9),rdn.Next(0,MainScreen.Ysize-9),rdn.Next(3,7),rdn.Next(3,7),rdn.Next(-1,2),rdn.Next(-1,2));
        }

        private void NewPopulation(Ero firstParent,Ero secondParent)
        {
            
            while(listEntities.Count() > 0)
            {
                RemoveEntity(listEntities[0]);
            }

            AddEro(firstParent);
            AddEro(secondParent);

            for(int i = 2; i < populationSize;i++)
            {
                AddEro(firstParent,secondParent);
            }

            this.AddEnemy(rdn.Next(0,MainScreen.Xsize-9),rdn.Next(0,MainScreen.Ysize-9),rdn.Next(3,7),rdn.Next(3,7),rdn.Next(-1,2),rdn.Next(-1,2));
            
        }

        public void Run()
        {
            long currentStep = 0;
            while(true)
            {
                currentStep++;
                MainScreen.Display();

                if(listEros.All(e => !(e.alive)))
                {
                    List<Ero> Survivors = listEros.OrderByDescending(e => e.score).Take(2).ToList();
                    NewPopulation(Survivors[0],Survivors[1]);
                    currentStep = 0;
                }

                foreach( Nemie entity in listNemies )
                {
                    entity.Move();

                    if( (entity.XPosition + entity.Xsize > MainScreen.Xsize) || ( entity.XPosition < 0 ) )
                    {
                        entity.Redirect(-entity.XMovement,entity.YMovement);
                    }

                    if( (entity.YPosition + entity.Ysize > MainScreen.Ysize) || ( entity.YPosition < 0 ) )
                    {
                        entity.Redirect(entity.XMovement,-entity.YMovement);
                    }

                    if(entity.XPosition + entity.Xsize >= MainScreen.Xsize)
                    {
                        entity.Teleport(MainScreen.Xsize-entity.Xsize,entity.YPosition);
                    }
                    else if( entity.XPosition < 0 )
                    {
                        entity.Teleport(0,entity.YPosition);
                    }

                    if(entity.YPosition + entity.Ysize >= MainScreen.Ysize)
                    {
                        entity.Teleport(entity.XPosition,MainScreen.Ysize-entity.Ysize);
                    }
                    else if ( entity.YPosition < 0 )
                    {
                        entity.Teleport(entity.XPosition,0);
                    }

                    MainScreen.AddObject(entity);
                }

                Task[] MovementDecision = new Task[listEros.Count()];
                for(int i = 0; i<listEros.Count(); i++)
                {
                    Ero currEro = listEros[i];
                    MovementDecision[i] = Task.Run(() => currEro.MakeAMove());
                }

                Task.WaitAll(MovementDecision);

                foreach( Ero entity in listEros )
                {
                    entity.Move();
                    
                    if(entity.XPosition + entity.Xsize > MainScreen.Xsize)
                    {
                        entity.Teleport(MainScreen.Xsize-entity.Xsize,entity.YPosition);
                    }
                    else if( entity.XPosition < 0 )
                    {
                        entity.Teleport(0,entity.YPosition);
                    }

                    if(entity.YPosition + entity.Ysize > MainScreen.Ysize)
                    {
                        entity.Teleport(entity.XPosition,MainScreen.Ysize-entity.Ysize);
                    }
                    else if ( entity.YPosition < 0 )
                    {
                        entity.Teleport(entity.XPosition,0);
                    }


                    if(entity.alive)
                    {
                        foreach(Nemie nemie in listNemies)
                        {
                            bool dead = !(entity.XPosition+(entity.Xsize) <= nemie.XPosition ||
                                        nemie.XPosition+(nemie.Xsize) <= entity.XPosition  ||
                                        entity.YPosition+(entity.Ysize) <= nemie.YPosition ||
                                        nemie.YPosition+(nemie.Ysize) <= entity.YPosition) ;
                            if(dead)
                            {
                                entity.Kill();
                                break;
                            }
                            
                        }
                        
                    }
                }

                MainScreen.AddObject(listEros.OrderByDescending(e => e.score).First());

                if(currentStep == spawningTime)
                {
                    currentStep = 0;
                    this.AddEnemy(rdn.Next(0,MainScreen.Xsize-9),rdn.Next(0,MainScreen.Ysize-9),rdn.Next(3,5),rdn.Next(3,5),rdn.Next(-1,2),rdn.Next(-1,2));
                }

                Thread.Sleep(30);
            }
        }

        private void AddEro()
        {
            Ero newCommer = new Ero(0,0,this);
            this.listEros.Add(newCommer);
            this.listEntities.Add(newCommer);
        }

        private void AddEro(Ero copy)
        {
            Ero newCommer = new Ero(0,0,copy);
            this.listEros.Add(newCommer);
            this.listEntities.Add(newCommer);
        }

        private void AddEro(Ero firstParent,Ero secondParent)
        {
            Ero newCommer = new Ero(0,0,this,firstParent,secondParent);
            this.listEros.Add(newCommer);
            this.listEntities.Add(newCommer);
        }


        private void AddEnemy(int XStartPos,int YStartPos,int XSize,int YSize,int XMovement= 1,int YMovement= 0)
        {
            Nemie newCommer = new Nemie( XStartPos, YStartPos, XSize, YSize, XMovement, YMovement);
            this.listNemies.Add(newCommer);
            this.listEntities.Add(newCommer);
        }

        private void RemoveEntity(BaseEntity DeadOne)
        {
            listEntities.Remove(DeadOne);
            
            if(DeadOne.GetType() == typeof(Ero)){
                listEros.Remove((Ero)DeadOne);
            }

            if(DeadOne.GetType() == typeof(Nemie)){
                listNemies.Remove((Nemie)DeadOne);
            }
        }

    }
}