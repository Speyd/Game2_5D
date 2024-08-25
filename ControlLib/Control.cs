using EntityLib;

namespace ControlLib
{
    public class Control
    {
        private CheckPressed checkPressed = new CheckPressed();

        private double moveSpeed;
        private readonly int speedMultiplier;

        public Control(int screenWidth, int screenHeight, double moveSpeed = 0.009)
        {
            this.moveSpeed = moveSpeed;
            speedMultiplier = screenWidth / 100;
        }

        private void forwardPress(Entity entity)
        {
            entity.EntityX += Math.Sin(entity.EntityA) * speedMultiplier * moveSpeed;
            entity.EntityY += Math.Cos(entity.EntityA) * speedMultiplier * moveSpeed;
        }
        private void backPress(Entity entity)
        {
            entity.EntityX -= Math.Sin(entity.EntityA) * speedMultiplier * moveSpeed;
            entity.EntityY -= Math.Cos(entity.EntityA) * speedMultiplier * moveSpeed;
        }
        private void leftPress(Entity entity)
        {
            entity.EntityX += moveSpeed * Math.Sin(entity.EntityA + Math.PI / 2);
            entity.EntityY += moveSpeed * Math.Cos(entity.EntityA + Math.PI / 2);
        }
        private void rightPress(Entity entity)
        {
            entity.EntityX -= moveSpeed * Math.Sin(entity.EntityA + Math.PI / 2);
            entity.EntityY -= moveSpeed * Math.Cos(entity.EntityA + Math.PI / 2);
        }
        private void turnLeftPress(Entity entity)
        {
            entity.EntityA += moveSpeed;
        }
        private void turnRightPress(Entity entity)
        {
            entity.EntityA -= moveSpeed;
        }

        public void makePressed(Entity entity)
        {
            checkPressed.check();

            if (checkPressed.isForwardPressed)
                forwardPress(entity);
            if (checkPressed.isBackPressed)
                backPress(entity);
            if (checkPressed.isLeftPressed)
                leftPress(entity);
            if (checkPressed.isRightPressed)
                rightPress(entity);
            if (checkPressed.isTurnLeftPressed)
                turnLeftPress(entity);
            if (checkPressed.isTurnRightPressed)
                turnRightPress(entity);
        }
    }
}
