
namespace Tests.Metadata;

/// <summary>
/// This has a public property that shadows another public property
/// </summary>
class ShadowedMock : ShadowedMockBase
{
	public new int SimpleProperty { get; set; }
	public new int SimpleProperty2 { get; set; }
}
