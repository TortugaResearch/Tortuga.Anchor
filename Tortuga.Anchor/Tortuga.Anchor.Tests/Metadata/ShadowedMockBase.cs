namespace Tests.Metadata;

class ShadowedMockBase : ShadowedMockBase2
{
	public int SimpleProperty { get; set; }
}

class ShadowedMockBase2 : ShadowedMockBase3
{
	public new string SimpleProperty2 { get; set; } = null!;
}

class ShadowedMockBase3
{
	public string SimpleProperty2 { get; set; } = null!;
}
