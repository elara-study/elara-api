namespace Elara.Application.Features.Users.Teachers.Queries.GetModuleResources
{
    public class ModuleResourcesDto
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public List<ResourceItemDto> Videos { get; set; } = new();
        public List<ResourceItemDto> Pdfs { get; set; } = new();
        public List<ResourceItemDto> Images { get; set; } = new();
    }

    public class ResourceItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string? SizeOrDuration { get; set; }
    }
}
