using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Utils
{
    public interface IAudio
    {
        void PlayAudioFileFromResource(string fileName);

        void PlayAudioFileFromFile(string fileName);

        void Pause();
        void Stop();
        void Resume();
    }
}
