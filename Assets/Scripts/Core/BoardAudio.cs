using UnityEngine;

namespace Assets.Scripts
{
    public class BoardAudio : MonoBehaviour
    {
        private AudioSource audioSource;
        private AudioClip boardContactSound;
        private AudioClip detailContactSound;
        [SerializeField] private AudioClip destroySound;


        void Start()
        {
            audioSource = this.GetComponent<AudioSource>();

            var gameManager = GameManager.Instance;
            if (gameManager == null || gameManager.BlockSkin == null || gameManager.BoardSkin == null)
                return;

            boardContactSound = gameManager.BoardSkin.Audio;
            detailContactSound = gameManager.BlockSkin.Audio;
        }
        
        public void PlayContactSound (int totalContacts, float colSum, bool isFloor, bool isHardDropping)
        {
            AudioClip audioClip;
            if(isFloor)
                audioClip = boardContactSound;
            else
                audioClip = detailContactSound;
            
            var panStereo = PanStereo(colSum/totalContacts);
            float volume = Mathf.Lerp(0.7f, 1f, (totalContacts - 1) / 3f);
            PlayClip(audioClip, panStereo, isHardDropping, volume);
        }
    
        public void PlayDestroySound(float col)
        {
            var panStereo = PanStereo(col);
            PlayClip(destroySound, panStereo, false, 0.3f);
        }

        private void PlayClip(AudioClip audioClip, float panStereo, bool isHardDropping, float volume = 1f)
        {
            audioSource.panStereo = panStereo;
            audioSource.pitch = isHardDropping ? 0.9f : 1f;
            audioSource.PlayOneShot(audioClip, volume);
        }

        private float PanStereo(float column)
        {
            var centrContact = Mathf.InverseLerp(0f, 9f, column);
            var panStereo = Mathf.Lerp(-0.5f, 0.5f, centrContact);
            return panStereo;
        }
    }
}
