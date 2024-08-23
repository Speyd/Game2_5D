namespace ControlLib
{
    public class Control
    {
        private CheckPressed checkPressed = new CheckPressed();

        private double moveSpeed;

        public Control(double moveSpeed = 0.009)
        {
            this.moveSpeed = moveSpeed;
        }

        private void forwardPress(ref double playerX, ref double playerY, ref double playerA)
        {
            playerX += Math.Sin(playerA) * moveSpeed;
            playerY += Math.Cos(playerA) * moveSpeed;
        }
        private void backPress(ref double playerX, ref double playerY, ref double playerA)
        {
            playerX -= Math.Sin(playerA) * moveSpeed;
            playerY -= Math.Cos(playerA) * moveSpeed;
        }
        private void leftPress(ref double playerX, ref double playerY, ref double playerA)
        {
            playerX += moveSpeed * Math.Sin(playerA + Math.PI / 2);
            playerY += moveSpeed * Math.Cos(playerA + Math.PI / 2);
        }
        private void rightPress(ref double playerX, ref double playerY, ref double playerA)
        {
            playerX -= moveSpeed * Math.Sin(playerA + Math.PI / 2);
            playerY -= moveSpeed * Math.Cos(playerA + Math.PI / 2);
        }
        private void turnLeftPress(ref double playerA)
        {
            playerA += moveSpeed / 2;
        }
        private void turnRightPress(ref double playerA)
        {
            playerA -= moveSpeed / 2;
        }

        public void makePressed(ref double playerX, ref double playerY, ref double playerA)
        {
            checkPressed.check();

            if (checkPressed.isForwardPressed)
                forwardPress(ref playerX, ref playerY, ref playerA);
            if (checkPressed.isBackPressed)
                backPress(ref playerX, ref playerY, ref playerA);
            if (checkPressed.isLeftPressed)
                leftPress(ref playerX, ref playerY, ref playerA);
            if (checkPressed.isRightPressed)
                rightPress(ref playerX, ref playerY, ref playerA);
            if (checkPressed.isTurnLeftPressed)
                turnLeftPress(ref playerA);
            if (checkPressed.isTurnRightPressed)
                turnRightPress(ref playerA);
        }
    }
}
