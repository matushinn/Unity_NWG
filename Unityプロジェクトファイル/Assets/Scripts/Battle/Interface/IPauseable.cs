//ポーズ可能なもの
namespace Battle
{
    public interface IPauseable
    {
        bool IsPaused { get; }

        void OnPause();

        void OnResume();
    }
}