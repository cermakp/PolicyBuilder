﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using Oriflame.PolicyBuilder.Policies.Builders.Enums;
using Oriflame.PolicyBuilder.Policies.Builders.Fluent.Attributes;
using Oriflame.PolicyBuilder.Policies.Builders.Fluent.Sections;
using Oriflame.PolicyBuilder.Policies.Definitions;
using Oriflame.PolicyBuilder.Xml.Builders.Attributes;
using Oriflame.PolicyBuilder.Xml.Definitions.Common;
using Oriflame.PolicyBuilder.Xml.Definitions.Inner;

namespace Oriflame.PolicyBuilder.Xml.Builders.Sections
{
    public abstract class SectionBuilderBase<TSection> : IPolicySectionBuilder<TSection> where TSection : class, IPolicySectionBuilder
    {
        protected readonly ISectionPolicy SectionPolicy;

        protected SectionBuilderBase(ISectionPolicy sectionPolicy)
        {
            SectionPolicy = sectionPolicy;
        }

        /// <inheritdoc />
        public virtual ISectionPolicy Create()
        {
            return SectionPolicy;
        }

        /// <inheritdoc />
        public virtual TSection Base()
        {
            return AddPolicyDefinition(new BasePolicy());
        }

        /// <inheritdoc />
        public virtual TSection Comment(string comment)
        {
            return AddPolicyDefinition(new CommentPolicy(comment));
        }

        /// <inheritdoc />
        public virtual TSection SetVariable(string name, string value)
        {
            return AddPolicyDefinition(new SetVariablePolicy(name, value));
        }

        /// <inheritdoc />
        public virtual ISectionPolicy ReturnResponse(Func<IReturnResponseSectionBuilder, ISectionPolicy> returnResponseBuilder)
        {
            var returnRequestSectionBuilder = new ReturnResponseSectionBuilder();
            AddPolicyDefinition(returnResponseBuilder.Invoke(returnRequestSectionBuilder) as IXmlPolicy);
            return Create();
        }

        /// <inheritdoc />
        public virtual TSection Trace(string sourceName, string content, Severity? severity = null)
        {
            return AddPolicyDefinition(new Trace(sourceName, content, severity));
        }

        /// <inheritdoc />
        public virtual TSection CacheLookupValue(string key, string variable)
        {
            return AddPolicyDefinition(new CacheLookupValuePolicy(key, variable));
        }

        /// <inheritdoc />
        public virtual TSection SendRequest(Func<ISendRequestAttributesBuilder, IDictionary<string, string>> sendRequestAttributesBuilder, Func<ISendRequestSectionBuilder, ISectionPolicy> sendRequestBuilder)
        {
            var attributes = sendRequestAttributesBuilder.Invoke(new SendRequestAttributesBuilder());
            var sendRequestSectionBuilder = new SendRequestSectionBuilder(attributes);
            return AddPolicyDefinition(sendRequestBuilder.Invoke(sendRequestSectionBuilder));
        }

        /// <inheritdoc />
        public virtual TSection CacheStoreValue(string key, string value, TimeSpan duration)
        {
            return AddPolicyDefinition(new CacheStoreValue(key, value, duration));
        }

        /// <inheritdoc />
        public virtual TSection Choose(Func<IConditionSectionBuilder<TSection>, ISectionPolicy> conditionBuilder)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual TSection SetHeader(string name, string value, ExistsAction? existsAction)
        {
            return AddPolicyDefinition(new SetHeaderParameter(name, value, existsAction));
        }

        /// <inheritdoc />
        public virtual TSection SetMethod(HttpMethod httpMethod)
        {
            return AddPolicyDefinition(new SetMethod(httpMethod));
        }

        /// <inheritdoc />
        public virtual TSection RateLimitByKey(int calls, int renewalPeriod, string counterKey)
        {
            return AddPolicyDefinition(new RateLimitByKey(calls, renewalPeriod, counterKey));
        }

        /// <inheritdoc />
        public virtual TSection RateLimitByKey(string calls, string renewalPeriod, string counterKey)
        {
            return AddPolicyDefinition(new RateLimitByKey(calls, renewalPeriod, counterKey));
        }

        /// <inheritdoc />
        public virtual TSection QuotaByKey(int calls, int bandwidth, int renewalPeriod, string counterKey)
        {
            return AddPolicyDefinition(new QuotaByKey(calls, bandwidth, renewalPeriod, counterKey));
        }

        /// <inheritdoc />
        public virtual TSection QuotaByKey(string calls, string bandwidth, string renewalPeriod, string counterKey)
        {
            return AddPolicyDefinition(new QuotaByKey(calls, bandwidth, renewalPeriod, counterKey));
        }

        protected TSection AddPolicyDefinition(IXmlPolicy policy)
        {
            SectionPolicy.AddInnerPolicy(policy);
            return Return();
        }

        protected TSection AddPolicyDefinition(ISectionPolicy policy)
        {
            SectionPolicy.AddInnerPolicy(policy as IXmlPolicy);
            return Return();
        }

        private TSection Return()
        {
            return this as TSection;
        }
    }
}
