using System.ComponentModel;

namespace Tests.Mocks;

public class MockChangeTracking : IRevertibleChangeTracking
{
	public bool IsChanged { get; set; }

	public void AcceptChanges() { IsChanged = false; }
	public void RejectChanges() { IsChanged = false; }
}
