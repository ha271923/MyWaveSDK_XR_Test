using Wave.Essence;

public interface IMECallbackIF {
	void InputDoneCallbackImpl(IMEManagerWrapper.InputResult results);
	void InputClickCallbackImpl(IMEManagerWrapper.InputResult results);
}