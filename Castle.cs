﻿namespace SFMLWWC
{
    internal class Castle
    {
        public const int WIDTH = 8;
        public const int HEIGHT = 8;
        public const int DEPTH = 50;
        private Random random;
        private Room[,,] rooms;

        public Castle()
        {
            random = new Random();

            rooms = new Room[WIDTH, HEIGHT, DEPTH];

            // Empty all the rooms
            for (var z = 0; z < DEPTH; z++)
                for (var y = 0; y < HEIGHT; y++)
                    for (var x = 0; x < WIDTH; x++)
                    {
                        var room = new Room();
                        rooms[x, y, z] = room;
                    }

            // Set the stairs down
            for (var z = 0; z < DEPTH; z++)
            {
                var x = random.NextInt64(WIDTH);
                var y = random.NextInt64(HEIGHT);

                var room = rooms[x, y, z];
                room.Items.Add(new Item(Content.StairsDown, 0));
            }

            // Set stairs up
            for (var z = 1; z < DEPTH - 1; z++)
            {
                var room = GetEmptyRoom(z);
                room.Items.Add(new Item(Content.StairsUp, 0));
            }

            // Gold!
            for(var z = 0; z < DEPTH; z++)
            {
                var room = GetEmptyRoom(z);
                room.Items.Add(new Item(Content.Gold, 100));
            }
        }

        public Room GetRoom(int x, int y, int z)
        {
            return rooms[x, y, z];
        }

        public Content GetRoomContents(int x, int y, int z)
        {
            var room = GetRoom(x, y, z);
            if (!room.Items.Any())
                return Content.Empty;

            return room.Items[0].Contents;
        }

        public Content GetRoomContents(Actor player)
        {
            return GetRoomContents(player.X, player.Y, player.Z);
        }

        public bool GetVisible(int x, int y, int z)
        {
            var room = GetRoom(x, y, z);
            return room.Visited;
        }

        public void Update(Actor player)
        {
            var room = GetRoom(player.X, player.Y, player.Z);
            room.Visited = true;

            if (!room.Items.Any())
                return;

            var list = new List<Item>();

            foreach (var item in room.Items)
            {
                switch (item.Contents)
                {
                    case Content.Gold:
                        PickupItem(room, player);
                        list.Add(item);
                        break;
                }
            }

            foreach (var item in list)
                room.Items.Remove(item);
        }

        private Room GetEmptyRoom(int z)
        {
            while(true)
            {
                var x = random.NextInt64(WIDTH);
                var y = random.NextInt64(HEIGHT);

                var room = rooms[x, y, z];
                if (!room.Items.Any())
                {
                    return room;
                }
            }
        }

        private void PickupItem(Room room, Actor actor)
        {
            foreach(var item in room.Items)
            {
                switch(item.Contents)
                {
                    case Content.Gold:
                        actor.Gold = actor.Gold + (int)item.Value;
                        break;
                }
            }
        }
    }
}
