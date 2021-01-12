using System;
using System.Text.Json.Serialization;

namespace Kartrider_Parts_Crawler.Models
{
    /// <summary>
    /// 파츠 타입
    /// </summary>
    public enum PartsType
    {
        // 엔진패치
        Engine,
        // 바퀴
        Wheel,
        // 킷
        Kit,
        // 핸들
        Handle,
    }
    public class Parts : IEquatable<Parts>
    {
        /// <summary>
        /// 파츠의 타입
        /// </summary>
        //JsonIgnore인 이유는 각 파츠타입별로 Json 파일을 생성하기 때문에 필요 없음
        [JsonIgnore]
        public PartsType PartsType { get; set; }
        /// <summary>
        /// 키, API 리스폰 데이터에 접근할 때 이것을 사용합니다.
        /// </summary>
        public int Key { get; set; }
        /// <summary>
        /// 파츠 이름
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 파츠 장착시 효과
        /// </summary>
        public string Effect { get; set; }
        /// <summary>
        /// 파츠 설명
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 파츠 이미지 URL
        /// </summary>
        public string ImageUrl { get; set; }

        public bool Equals(Parts other)
        {
            if (other is null) return false;

            if (Object.ReferenceEquals(this, other)) return true;

            return Key.Equals(other.Key) && Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            int hashProductCode = Name.GetHashCode();

            return Key ^ hashProductCode;
        }
    }
}
