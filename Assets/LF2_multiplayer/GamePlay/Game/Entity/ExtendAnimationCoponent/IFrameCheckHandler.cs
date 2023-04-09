using LF2;

public interface IFrameCheckHandler {
        // void onHitFrameStart();
        // void onHitFrameEnd();
        // void onLastFrameStart();
        // void playAnimation(int frame);
        void playSound(AudioCueSO audioCue);

        void onMoveFrame(int nextFrame);
        void onAttackFrame(AttackDataSend attackDataSend);
}