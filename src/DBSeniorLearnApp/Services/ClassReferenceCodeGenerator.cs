using SCG = System.Collections.Generic;

namespace DBSeniorLearnApp.Services;

public class ClassReferenceCodeGenerator {
	
	public static SCG::Dictionary<int/*CourseType type*/, string> classtypes = new SCG::Dictionary<int, string>();
	
	public static string New(/*CourseType type*/) {
		throw new System.NotImplementedException();
	}
}
