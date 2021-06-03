using Wave.Essence;

namespace Hubble.Launcher.Input
{
	public interface IMECallbackIF
	{
		void InputDoneCallbackImpl(IMEManagerWrapper.InputResult results);
		void InputClickCallbackImpl(IMEManagerWrapper.InputResult results);
	}
}